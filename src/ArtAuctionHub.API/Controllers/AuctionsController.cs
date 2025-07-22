using ArtAuctionHub.Application.DTOs.Auction;
using ArtAuctionHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The AuctionsController handles operations related to auctions,
    /// such as starting, editing, deleting, and retrieving auction details.
    /// </summary>
    /// <param name="_auctionService"> An instance of IAuctionService to handle auction operations. Registered in the Dependency Injection (DI) container.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/auctions")] // Base route for all auction-related endpoints. (/api/auctions)
    public class AuctionsController(IAuctionService _auctionService) : ControllerBase
    {
        /// <summary>
        /// Starts a new auction for a given artwork.
        /// </summary>
        /// <param name="dto">The AuctionDto containing auction details (start/end date, artwork ID).</param>
        /// <returns>A 201 Created response with the created auction data.</returns>
        [HttpPost] // Matches POST /api/auctions.
        public IActionResult StartAuction([FromBody] AuctionDto dto)
        {
            // Future implementation:
            // 1. Validate the auction details.
            // 2. Link auction to artwork and save it.
            var createdAuction = _auctionService.CreateAuction(dto);
            return Ok(createdAuction);
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
            // Future implementation:
            // 1. Check if the auction with the given ID exists.
            // 2. Update its properties based on the provided dto.
            // 3. Save changes to the database.
            var updatedAuction = _auctionService.UpdateAuction(id, dto);
            return Ok(updatedAuction);
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
            // Future implementation:
            // 1. Check if the auction with the given ID exists.
            // 2. Delete it from the database.
            _auctionService.DeleteAuction(id);
            return NoContent(); // Returns 204 No Content on successful deletion.
        }

        /// <summary>
        /// Retrieves all active auctions.
        /// </summary>
        /// <returns>A 200 OK response with active auctions.</returns>
        [HttpGet]
        [Route("active")] // Matches GET /api/auctions/active.
        public IActionResult GetActiveAuctions()
        {
            // Future implementation:
            // 1. Fetch active auctions from the database.
            // 2. Return them in a suitable format (e.g., list of auction DTOs).
            var activeAuctions = _auctionService.GetActiveAuctions();
            return Ok(activeAuctions);
        }

        /// <summary>
        /// Retrieves auctions created by the current user.
        /// </summary>
        /// <returns>A 200 OK response with user's auctions.</returns>
        [HttpGet]
        [Route("my")] // Matches GET /api/auctions/my.
        public IActionResult GetMyAuctions()
        {
            // Future implementation:
            // 1. Fetch auctions created by the authenticated user.
            // 2. Return them in a suitable format (e.g., list of auction DTOs).
            var myAuctions = _auctionService.GetUserAuctions();
            return Ok(myAuctions);
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
            // Future implementation:
            // 1. Fetch the auction details by ID from the database.
            // 2. Return the auction data in a suitable format (e.g., auction DTO).
            var auctionDetails = _auctionService.GetAuctionById(id);
            if (auctionDetails == null)
            {
                return NotFound(); // Returns 404 Not Found if the auction does not exist.
            }
            return Ok(auctionDetails);
        }
    }
}