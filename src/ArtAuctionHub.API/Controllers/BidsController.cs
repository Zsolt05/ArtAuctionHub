using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Shared.Constants;
using ArtAuctionHub.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The BidsController handles placing and retrieving bids on auctions.
    /// </summary>
    /// <param name="_bidService">An instance of IBidService to handle bid operations. Registered in the Dependency Injection (DI) container.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/bids")] // Base route for all bid-related endpoints. (/api/bids/place)
    [Authorize]
    public class BidsController(IBidService _bidService) : ControllerBase
    {
        /// <summary>
        /// Places a bid on an auction.
        /// </summary>
        /// <param name="dto">The BidDto containing auction ID and bid amount.</param>
        /// <returns>A 200 OK response with confirmation of the bid.</returns>
        [HttpPost] // Matches POST /api/bids/place.
        [Authorize(Roles = RoleNames.Buyer)] // Only users with the "Buyer" role can access this endpoint.
        public async Task<IActionResult> PlaceBid([FromBody] BidDto dto)
        {
            int userId = User.GetUserId();
            await _bidService.PlaceBidAsync(dto, userId);
            return Ok(new { Message = "Bid placed successfully." });
        }

        /// <summary>
        /// Retrieves all bids made by the current user.
        /// </summary>
        /// <returns>A 200 OK response with user's bids.</returns>
        [HttpGet]
        [Route("my")] // Matches GET /api/bids/my.
        [Authorize(Roles = RoleNames.Buyer)] // Only users with the "Buyer" role can access this endpoint.
        public async Task<IActionResult> GetMyBids()
        {
            int userId = User.GetUserId();
            var myBids = await _bidService.GetBidsForUserAsync(userId);
            return Ok(myBids); // Returns 200 OK with the user's bids.
        }
    }
}