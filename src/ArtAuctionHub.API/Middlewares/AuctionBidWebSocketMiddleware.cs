using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Shared.Exceptions;
using ArtAuctionHub.Shared.Extensions;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ArtAuctionHub.API.Middlewares
{
    /// <summary>
    /// WebSocket middleware for real-time auction bid handling.
    /// Handles client connections, bid placement, and broadcasting bid updates to all connected clients for a given auction.
    /// </summary>
    public class AuctionBidWebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Stores active WebSocket connections for each auction.
        /// Key: auctionId, Value: dictionary of WebSockets for that auction.
        /// </summary>
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<WebSocket, byte>> _auctionSockets = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionBidWebSocketMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public AuctionBidWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Processes incoming HTTP requests and handles WebSocket connections for auction bids.
        /// If the request is a WebSocket upgrade for auction bids, authenticates the user, validates the auction, and manages the WebSocket lifecycle.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request is for the auction bids WebSocket endpoint
            if (context.Request.Path == "/ws/auction-bids" && context.WebSockets.IsWebSocketRequest)
            {
                // Ensure the user is authenticated
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                // Parse auctionId from query string
                var auctionIdStr = context.Request.Query["auctionId"].ToString();
                if (string.IsNullOrWhiteSpace(auctionIdStr) || !int.TryParse(auctionIdStr, out var auctionId))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                // Resolve the bid service from DI
                using var scope = context.RequestServices.CreateScope();
                var bidService = scope.ServiceProvider.GetRequiredService<IBidService>();

                Bid? currentHighestBid;
                try
                {
                    // Get the current highest bid for the auction
                    currentHighestBid = await bidService.GetHighestBidForAuctionAsync(auctionId);
                    if (currentHighestBid == null)
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }
                }
                catch (NotFoundException ex)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }

                // Accept the WebSocket connection
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                // Add the new WebSocket to the list for this auction
                var sockets = _auctionSockets.GetOrAdd(auctionId, _ => new ConcurrentDictionary<WebSocket, byte>());
                sockets.TryAdd(webSocket, 0);

                // Send the current highest bid to the new client
                var bidMsg = new
                {
                    type = "currentBid",
                    auctionId,
                    amount = currentHighestBid.Amount,
                    userId = currentHighestBid.UserId
                };
                var bidJson = JsonSerializer.Serialize(bidMsg);
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(bidJson), WebSocketMessageType.Text, true, CancellationToken.None);

                try
                {
                    // Start the bid handling loop for this client
                    await AuctionBidLoop(webSocket, auctionId, bidService, context);
                }
                finally
                {
                    // Remove the WebSocket when done (client disconnects or error)
                    RemoveSocket(auctionId, webSocket);
                }
            }
            else
            {
                // Not a WebSocket request for auction bids, continue to next middleware
                await _next(context);
            }
        }

        /// <summary>
        /// Handles the WebSocket loop for receiving and processing bid messages from a client.
        /// Validates and processes incoming bids, broadcasts updates to all clients, and handles errors.
        /// </summary>
        /// <param name="webSocket">The WebSocket connection.</param>
        /// <param name="auctionId">The auction identifier.</param>
        /// <param name="bidService">The bid service instance.</param>
        /// <param name="context">The HTTP context.</param>
        private static async Task AuctionBidLoop(WebSocket webSocket, int auctionId, IBidService bidService, HttpContext context)
        {
            var buffer = new byte[4096];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                try
                {
                    // Wait for a message from the client
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                catch
                {
                    // Connection error, exit loop
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    // Client requested to close the connection
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                }
                else
                {
                    // Read the received message as text
                    var receivedText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    try
                    {
                        // Deserialize the message to determine its type and content
                        var message = JsonSerializer.Deserialize<WebSocketBidMessage>(receivedText,
                            options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // Handle a bid placement request
                        if (message?.Type == "placeBid")
                        {
                            // Get the current highest bid for validation
                            var highestBidResult = await bidService.GetHighestBidForAuctionAsync(auctionId);

                            // Reject if the new bid is not higher than the current highest
                            if (highestBidResult != null && message.Amount <= highestBidResult.Amount)
                            {
                                var error = new { type = "error", message = $"Bid amount must be higher than the current highest bid of {highestBidResult.Amount}." };
                                var errorJson = JsonSerializer.Serialize(error);
                                await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorJson), WebSocketMessageType.Text, true, CancellationToken.None);
                                continue;
                            }

                            int userId;
                            try
                            {
                                // Get the user ID from the context
                                userId = context.User.GetUserId();
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                // User is not authorized, send error
                                var error = new { type = "error", message = ex.Message };
                                var errorJson = JsonSerializer.Serialize(error);
                                await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorJson), WebSocketMessageType.Text, true, CancellationToken.None);
                                continue;
                            }

                            // Create and place the new bid
                            var newBid = new Bid
                            {
                                AuctionId = auctionId,
                                Amount = message.Amount
                            };
                            await bidService.PlaceBidAsync(newBid, userId);

                            // Prepare the response to broadcast to all clients
                            var response = new
                            {
                                type = "bidPlaced",
                                auctionId,
                                amount = newBid.Amount,
                                userId,
                                status = true,
                                message = $"Bid of {newBid.Amount} placed on auction {auctionId}."
                            };
                            var responseJson = JsonSerializer.Serialize(response);
                            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                            // Broadcast the new bid to all connected clients for this auction
                            if (_auctionSockets.TryGetValue(auctionId, out var sockets))
                            {
                                var tasks = new List<Task>();
                                foreach (var s in sockets.Keys)
                                {
                                    if (s.State == WebSocketState.Open)
                                    {
                                        tasks.Add(SendAndHandleSocket(s, responseBytes, auctionId));
                                    }
                                    else
                                    {
                                        // Remove closed sockets
                                        RemoveSocket(auctionId, s);
                                    }
                                }
                                await Task.WhenAll(tasks);
                            }
                        }
                        else
                        {
                            // Unknown message type, send error
                            var error = new { type = "error", message = "Unknown message type." };
                            var errorJson = JsonSerializer.Serialize(error);
                            await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorJson), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    catch (NotFoundException ex)
                    {
                        // Auction not found, send error and close connection
                        var error = new { type = "error", message = ex.Message };
                        var errorJson = JsonSerializer.Serialize(error);
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorJson), WebSocketMessageType.Text, true, CancellationToken.None);
                        return;
                    }
                    catch
                    {
                        // Invalid message format, send error
                        var error = new { type = "error", message = "Invalid message format." };
                        var errorJson = JsonSerializer.Serialize(error);
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorJson), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a message to the specified WebSocket and removes the socket if an error occurs (e.g., client disconnected).
        /// </summary>
        /// <param name="socket">The WebSocket connection.</param>
        /// <param name="message">The message bytes to send.</param>
        /// <param name="auctionId">The auction identifier.</param>
        private static async Task SendAndHandleSocket(WebSocket socket, byte[] message, int auctionId)
        {
            try
            {
                await socket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch
            {
                // If sending fails, remove the socket from the list
                RemoveSocket(auctionId, socket);
            }
        }

        /// <summary>
        /// Removes a WebSocket from the list of sockets for the specified auction.
        /// </summary>
        /// <param name="auctionId">The auction identifier.</param>
        /// <param name="socket">The WebSocket connection to remove.</param>
        private static void RemoveSocket(int auctionId, WebSocket socket)
        {
            if (_auctionSockets.TryGetValue(auctionId, out var sockets))
            {
                sockets.TryRemove(socket, out _);
            }
        }

        /// <summary>
        /// Represents a WebSocket bid message sent by the client.
        /// </summary>
        /// <example>
        /// {
        ///   "type": "placeBid",
        ///   "amount": 150.00
        /// }
        /// </example>
        private sealed record WebSocketBidMessage
        {
            /// <summary>
            /// The type of the message (e.g., "placeBid").
            /// </summary>
            public string? Type { get; init; } = default!;
            /// <summary>
            /// The bid amount.
            /// </summary>
            public decimal Amount { get; init; } = default!;
        }
    }
}