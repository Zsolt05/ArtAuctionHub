using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ArtAuctionHub.API.Hubs
{
    /// <summary>
    /// SignalR hub for real-time auction bid handling.
    /// </summary>
    [Authorize]
    public class AuctionBidHub(IBidService bidService) : Hub
    {
        /// <summary>
        /// Called by clients to join an auction group and receive real-time bid updates.
        /// </summary>
        public async Task JoinAuction(int auctionId)
        {
            // Validate auction exists
            var currentHighestBid = await bidService.GetHighestBidForAuctionAsync(auctionId);
            if (currentHighestBid == null)
            {
                throw new HubException("Auction not found.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GetAuctionGroup(auctionId));

            // Send current highest bid to the joining client
            await Clients.Caller.SendAsync("CurrentBid", new
            {
                auctionId,
                amount = currentHighestBid.Amount,
                userId = currentHighestBid.UserId
            });
        }

        /// <summary>
        /// Called by clients to place a bid.
        /// </summary>
        public async Task PlaceBid(int auctionId, decimal amount)
        {
            // Validate auction exists and get current highest bid
            var highestBid = await bidService.GetHighestBidForAuctionAsync(auctionId);
            if (highestBid == null)
            {
                await Clients.Caller.SendAsync("Error", $"Auction {auctionId} not found.");
                return;
            }

            if (amount <= highestBid.Amount)
            {
                await Clients.Caller.SendAsync("Error", $"Bid amount must be higher than the current highest bid of {highestBid.Amount}.");
                return;
            }

            int userId;
            try
            {
                userId = Context.User?.GetUserId() ??
                    throw new UnauthorizedAccessException("User ID not found in token.");
            }
            catch (UnauthorizedAccessException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
                return;
            }

            var newBid = new BidDto
            {
                AuctionId = auctionId,
                Amount = amount
            };

            await bidService.PlaceBidAsync(newBid, userId);

            // Broadcast the new bid to all clients in the auction group
            await Clients.Group(GetAuctionGroup(auctionId)).SendAsync("BidPlaced", new
            {
                auctionId,
                amount = newBid.Amount,
                userId,
                status = true,
                message = $"Bid of {newBid.Amount} placed on auction {auctionId}."
            });
        }

        private static string GetAuctionGroup(int auctionId) => $"auction_{auctionId}";
    }
}