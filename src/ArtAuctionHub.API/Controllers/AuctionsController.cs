using ArtAuctionHub.API.Extensions;
using ArtAuctionHub.Application.DTOs.Auction;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing auction operations such as 
    /// creating, updating, deleting, and retrieving auction details.
    /// All operations are asynchronous and rely on the IAuctionService.
    /// </summary>
    /// <remarks>
    /// Constructor that injects IAuctionService.
    /// </remarks>
    [ApiController]
    [Route("api/auctions")]
    [Authorize]
    public class AuctionsController(IAuctionService auctionService) : ControllerBase
    {
        /// <summary>
        /// Starts a new auction.
        /// </summary>
        /// <param name="dto">Auction data (artwork ID, start/end dates, starting price).</param>
        /// <returns>201 Created with the created auction data, or 400 BadRequest if the artwork does not exist.</returns>
        [HttpPost]
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> StartAuction([FromBody] AuctionDto dto)
        {
            int userId = User.GetUserId();
            var createdAuction = await auctionService.CreateAuctionAsync(userId,dto);
            return CreatedAtAction(nameof(GetAuctionDetails), new { id = dto.ArtworkId }, createdAuction);
        }

        /// <summary>
        /// Updates an existing auction.
        /// </summary>
        /// <param name="auctionId">ID of the auction to update.</param>
        /// <param name="dto">Updated auction data.</param>
        /// <returns>200 OK with the updated auction, 404 NotFound if the auction is missing, or 400 BadRequest if invalid data is provided.</returns>
        [HttpPut("{auctionId}")]
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> EditAuction(int auctionId, [FromBody] AuctionDto dto)
        {
            int userId = User.GetUserId();
            var updatedAuction = await auctionService.UpdateAuctionAsync(userId, auctionId, dto);
            return Ok(updatedAuction);
        }

        /// <summary>
        /// Deletes an auction by its ID.
        /// </summary>
        /// <param name="auctionId">ID of the auction to delete.</param>
        /// <returns>204 NoContent if deletion is successful, or 404 NotFound if the auction does not exist.</returns>
        [HttpDelete("{auctionId}")]
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> DeleteAuction(int auctionId)
        {
            int userId = User.GetUserId();
            await auctionService.DeleteAuctionAsync(userId, auctionId);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all currently active auctions.
        /// </summary>
        /// <returns>200 OK with a list of active auctions.</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAuctions()
        {
            var activeAuctions = await auctionService.GetActiveAuctionsAsync();
            return Ok(activeAuctions);
        }

        /// <summary>
        /// Retrieves all auctions created by the current user.
        /// (For now, returns all auctions without filtering by user.)
        /// </summary>
        /// <returns>200 OK with a list of auctions.</returns>
        [HttpGet("my")]
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> GetMyAuctions()
        {
            int userId = User.GetUserId();
            var myAuctions = await auctionService.GetUserAuctionsAsync(userId);
            return Ok(myAuctions);
        }

        /// <summary>
        /// Retrieves details of a specific auction by ID.
        /// </summary>
        /// <param name="id">ID of the auction.</param>
        /// <returns>200 OK with auction details, or 404 NotFound if the auction does not exist.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionDetails(int id)
        {
            var auctionDetails = await auctionService.GetAuctionByIdAsync(id);
            if (auctionDetails == null)
            {
                return NotFound(new { message = $"Auction with ID {id} not found." });
            }
            return Ok(auctionDetails);
        }
    }
}