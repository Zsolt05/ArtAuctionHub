using ArtAuctionHub.Application.DTOs.Auction;
using ArtAuctionHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing auction operations such as 
    /// creating, updating, deleting, and retrieving auction details.
    /// All operations are asynchronous and rely on the IAuctionService.
    /// </summary>
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        /// <summary>
        /// Constructor that injects IAuctionService.
        /// </summary>
        public AuctionsController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        /// <summary>
        /// Starts a new auction.
        /// </summary>
        /// <param name="dto">Auction data (artwork ID, start/end dates, starting price).</param>
        /// <returns>201 Created with the created auction data, or 400 BadRequest if the artwork does not exist.</returns>
        [HttpPost]
        public async Task<IActionResult> StartAuction([FromBody] AuctionDto dto)
        {
            try
            {
                var createdAuction = await _auctionService.CreateAuctionAsync(dto);
                return CreatedAtAction(nameof(GetAuctionDetails), new { id = dto.ArtworkId }, createdAuction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing auction.
        /// </summary>
        /// <param name="id">ID of the auction to update.</param>
        /// <param name="dto">Updated auction data.</param>
        /// <returns>200 OK with the updated auction, 404 NotFound if the auction is missing, or 400 BadRequest if invalid data is provided.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAuction(int id, [FromBody] AuctionDto dto)
        {
            try
            {
                var updatedAuction = await _auctionService.UpdateAuctionAsync(id, dto);
                return Ok(updatedAuction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an auction by its ID.
        /// </summary>
        /// <param name="id">ID of the auction to delete.</param>
        /// <returns>204 NoContent if deletion is successful, or 404 NotFound if the auction does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            try
            {
                await _auctionService.DeleteAuctionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all currently active auctions.
        /// </summary>
        /// <returns>200 OK with a list of active auctions.</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAuctions()
        {
            var activeAuctions = await _auctionService.GetActiveAuctionsAsync();
            return Ok(activeAuctions);
        }

        /// <summary>
        /// Retrieves all auctions created by the current user.
        /// (For now, returns all auctions without filtering by user.)
        /// </summary>
        /// <returns>200 OK with a list of auctions.</returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAuctions()
        {
            var myAuctions = await _auctionService.GetUserAuctionsAsync();
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
            var auctionDetails = await _auctionService.GetAuctionByIdAsync(id);
            if (auctionDetails == null)
            {
                return NotFound(new { message = $"Auction with ID {id} not found." });
            }
            return Ok(auctionDetails);
        }
    }
}