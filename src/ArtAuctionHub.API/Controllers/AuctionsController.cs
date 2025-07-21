using ArtAuctionHub.Application.DTOs.Auction;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The AuctionsController handles operations related to auctions,
    /// such as starting, editing, deleting, and retrieving auction details.
    /// </summary>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/auctions")] // Base route for all auction-related endpoints. (/api/auctions)
    public class AuctionsController : ControllerBase
    {
        /// <summary>
        /// Starts a new auction for a given artwork.
        /// </summary>
        /// <param name="dto">The AuctionDto containing auction details (start/end date, artwork ID).</param>
        /// <returns>A 201 Created response with the created auction data.</returns>
        [HttpPost] // Matches POST /api/auctions.
        public IActionResult StartAuction([FromBody] AuctionDto dto)
        {
            // Real implementation:
            // 1. Validate the auction details.
            // 2. Link auction to artwork and save it.
            return Created("", dto);
        }

        /// <summary>
        /// Edits an existing auction.
        /// </summary>
        /// <param name="id">The ID of the auction to edit.</param>
        /// <param name="dto">The updated auction details.</param>
        /// <returns>A 200 OK response with the updated auction.</returns>
        [HttpPut]
        [Route("{id}")] // Matches PUT /api/auctions/{id}.
        public IActionResult EditAuction(int id, [FromBody] AuctionDto dto)
        {
            return Ok(dto);
        }

        /// <summary>
        /// Deletes an auction by its ID.
        /// </summary>
        /// <param name="id">The ID of the auction.</param>
        /// <returns>A 204 No Content response when deleted.</returns>
        [HttpDelete]
        [Route("{id}")] // Matches DELETE /api/auctions/{id}.
        public IActionResult DeleteAuction(int id)
        {
            return NoContent();
        }

        /// <summary>
        /// Retrieves all active auctions.
        /// </summary>
        /// <returns>A 200 OK response with active auctions.</returns>
        [HttpGet]
        [Route("active")] // Matches GET /api/auctions/active.
        public IActionResult GetActiveAuctions()
        {
            return Ok(new[] { "Auction1", "Auction2" });
        }

        /// <summary>
        /// Retrieves auctions created by the current user.
        /// </summary>
        /// <returns>A 200 OK response with user's auctions.</returns>
        [HttpGet]
        [Route("my")] // Matches GET /api/auctions/my.
        public IActionResult GetMyAuctions()
        {
            return Ok(new[] { "MyAuction1" });
        }

        /// <summary>
        /// Retrieves details of a specific auction.
        /// </summary>
        /// <param name="id">The ID of the auction.</param>
        /// <returns>A 200 OK response with auction details.</returns>
        [HttpGet]
        [Route("{id}")] // Matches GET /api/auctions/{id}.
        public IActionResult GetAuctionDetails(int id)
        {
            return Ok(new { AuctionId = id, Status = "Active" });
        }
    }
}