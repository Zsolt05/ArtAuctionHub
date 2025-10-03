using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace ArtAuctionHub.SignalRClientTest
{
    /// <summary>
    /// Entry point for the SignalR client test application.
    /// This client demonstrates authentication, SignalR connection, group joining, and bid placement for testing the AuctionBidHub.
    /// </summary>
    class Program
    {
        private static readonly string ApiBaseUrl = "http://localhost:5000";
        private static readonly string HubUrl = $"{ApiBaseUrl}/hubs/auction-bids";

        static async Task Main(string[] args)
        {
            try
            {
                var jwt = await LoginAndGetJwtAsync("Buyer@test.com", "Buyer123!");
                if (string.IsNullOrWhiteSpace(jwt))
                {
                    Console.WriteLine("Login failed. Please check your credentials and API availability.");
                    return;
                }

                var connection = BuildSignalRConnection(jwt);
                RegisterSignalREventHandlers(connection);

                await connection.StartAsync();
                Console.WriteLine("SignalR connection started.");

                int auctionId = 4;
                if (!await TryJoinAuctionAsync(connection, auctionId))
                {
                    await connection.StopAsync();
                    return;
                }

                await RunInteractiveLoopAsync(connection, auctionId);

                await connection.StopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds and configures the SignalR connection.
        /// </summary>
        private static HubConnection BuildSignalRConnection(string jwt)
        {
            return new HubConnectionBuilder()
                .WithUrl(HubUrl, options =>
                {
                    options.AccessTokenProvider = () =>
                        Task.FromResult<string?>(string.IsNullOrWhiteSpace(jwt) ? string.Empty : jwt);
                })
                .WithAutomaticReconnect()
                .Build();
        }

        /// <summary>
        /// Registers handlers for SignalR events.
        /// </summary>
        private static void RegisterSignalREventHandlers(HubConnection connection)
        {
            connection.On<JsonElement>("CurrentBid", bid =>
            {
                PrintBid("CurrentBid", bid);
                PrintPrompt();
            });

            connection.On<JsonElement>("BidPlaced", bid =>
            {
                PrintBid("BidPlaced", bid);
                PrintPrompt();
            });

            connection.On<string>("Error", error =>
            {
                if (!string.IsNullOrWhiteSpace(error))
                    Console.WriteLine($"[Error] {error}");
                else
                    Console.WriteLine("[Error] (empty or null)");
                PrintPrompt();
            });
        }

        /// <summary>
        /// Prints bid information from a JsonElement.
        /// </summary>
        private static void PrintBid(string label, JsonElement bid)
        {
            try
            {
                if (bid.ValueKind == JsonValueKind.Object)
                {
                    var auctionId = bid.TryGetProperty("auctionId", out var auctionIdProp) ? auctionIdProp.GetInt32() : (int?)null;
                    var amount = bid.TryGetProperty("amount", out var amountProp) ? amountProp.GetDecimal() : (decimal?)null;
                    var userId = bid.TryGetProperty("userId", out var userIdProp) ? userIdProp.GetInt32() : (int?)null;
                    var status = bid.TryGetProperty("status", out var statusProp) ? statusProp.GetBoolean() : (bool?)null;
                    var message = bid.TryGetProperty("message", out var messageProp) ? messageProp.GetString() : null;

                    Console.Write($"[{label}] Auction: {auctionId}, Amount: {amount}, User: {userId}");
                    if (status.HasValue) Console.Write($", Status: {status}");
                    if (!string.IsNullOrWhiteSpace(message)) Console.Write($", Message: {message}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"[{label}] Not an object");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{label}] Error parsing bid: {ex.Message}");
            }
        }

        /// <summary>
        /// Prints the command prompt.
        /// </summary>
        private static void PrintPrompt()
        {
            Console.Write("> ");
        }

        /// <summary>
        /// Tries to join the specified auction group.
        /// </summary>
        private static async Task<bool> TryJoinAuctionAsync(HubConnection connection, int auctionId)
        {
            try
            {
                await connection.InvokeAsync("JoinAuction", auctionId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to join auction: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Runs the interactive command loop for placing bids or exiting.
        /// </summary>
        private static async Task RunInteractiveLoopAsync(HubConnection connection, int auctionId)
        {
            Console.WriteLine("Type 'bid' to place a new bid, or '\\q' to exit. New bids will be displayed automatically.");
            PrintPrompt();

            bool exitRequested = false;
            while (!exitRequested)
            {
                var command = Console.ReadLine();
                if (command is null)
                {
                    PrintPrompt();
                    continue;
                }

                command = command.Trim().ToLower();

                switch (command)
                {
                    case "\\q":
                        exitRequested = true;
                        break;
                    case "bid":
                        await HandleBidAsync(connection, auctionId);
                        break;
                    default:
                        PrintPrompt();
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the bid placement process.
        /// </summary>
        private static async Task HandleBidAsync(HubConnection connection, int auctionId)
        {
            Console.Write("> Enter bid amount: ");
            var amountInput = Console.ReadLine();
            if (decimal.TryParse(amountInput, out var amount))
            {
                try
                {
                    await connection.InvokeAsync("PlaceBid", auctionId, amount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to place bid: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid amount entered. Skipping bid.");
            }
            PrintPrompt();
        }

        /// <summary>
        /// Authenticates the user with the API and retrieves a JWT token.
        /// </summary>
        private static async Task<string?> LoginAndGetJwtAsync(string email, string password)
        {
            try
            {
                using var client = new HttpClient();
                var loginUrl = $"{ApiBaseUrl}/api/auth/login";
                var payload = new { email, password };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(loginUrl, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                dynamic? obj = JsonConvert.DeserializeObject(json);
                if (obj == null)
                    return null;

                var token = obj.token;
                if (token == null)
                    return null;

                if (token.accessToken != null)
                    return (string)token.accessToken;

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginAndGetJwtAsync exception: {ex.Message}");
                return null;
            }
        }
    }
}
