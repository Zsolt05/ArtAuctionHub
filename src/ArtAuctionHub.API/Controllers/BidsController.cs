using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The BidsController handles placing and retrieving bids on auctions.
    /// </summary>
    /// <param name="_bidService">An instance of IBidService to handle bid operations. Registered in the Dependency Injection (DI) container.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/bids")] // Base route for all bid-related endpoints. (/api/bids/place)
    public class BidsController(IBidService _bidService) : ControllerBase
    {
        /// <summary>
        /// Places a bid on an auction.
        /// </summary>
        /// <param name="dto">The BidDto containing auction ID and bid amount.</param>
        /// <returns>A 200 OK response with confirmation of the bid.</returns>
        [HttpPost] // Matches POST /api/bids/place.
        public async Task<IActionResult> PlaceBid([FromBody] BidDto dto)
        {
            int userId = 1; // Placeholder for the current user's ID. In a future implementation, this would be retrieved from the authenticated user's context.

            // Future implementation would involve:
            // 1. Validate the dto (ensure the bid amount is valid).
            // 2. Check if the auction exists and is active.
            // 3. Check user is buying (not an artist).
            // 4. Save the bid to the database.
            // 5. Return the created bid or a confirmation message.
            await _bidService.PlaceBidAsync(dto, userId);
            return Ok(new { Message = "Bid placed successfully." });
        }

        /// <summary>
        /// Retrieves all bids made by the current user.
        /// </summary>
        /// <returns>A 200 OK response with user's bids.</returns>
        [HttpGet]
        [Route("my")] // Matches GET /api/bids/my.
        public async Task<IActionResult> GetMyBids()
        {
            int userId = 1; // Placeholder for the current user's ID. In a future implementation, this would be retrieved from the authenticated user's context.

            // Future implementation would involve:
            // 1. Fetching bids from the database where the user is the buyer.
            // 2. Returning the list of bids in a suitable format (e.g., list of BidDto).
            var myBids = await _bidService.GetBidsForUserAsync(userId);
            return Ok(myBids); // Returns 200 OK with the user's bids.
        }
    }
}