using ArtAuctionHub.Application.DTOs.Auction;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Shared.Constants;
using ArtAuctionHub.Shared.Extensions;
using AutoMapper;
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
    /// Constructor that injects IAuctionService and ILogger.
    /// </remarks>
    [ApiController]
    [Route("api/auctions")]
    [Authorize]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly ILogger<AuctionsController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for AuctionsController.
        /// </summary>
        /// <param name="auctionService">Injected auction service.</param>
        /// <param name="logger">Logger for auction actions.</param>
        /// <param name="mapper">AutoMapper instance for DTO mapping.</param>
        public AuctionsController(IAuctionService auctionService,
            ILogger<AuctionsController> logger, IMapper mapper)
        {
            _auctionService = auctionService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Starts a new auction.
        /// </summary>
        /// <param name="dto">Auction data (artwork ID, start/end dates, starting price).</param>
        /// <returns>201 Created with the created auction data, or 400 BadRequest if the artwork does not exist.</returns>
        [HttpPost] // POST /api/auctions
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> StartAuction([FromBody] AuctionDto dto)
        {
            int userId = User.GetUserId();
            _logger.LogInformation("User {UserId} is starting a new auction for artwork {ArtworkId}", userId, dto.ArtworkId);
            var auction = _mapper.Map<Auction>(dto);
            var createdAuction = await _auctionService.CreateAuctionAsync(userId, auction);
            _logger.LogInformation("Auction created for artwork {ArtworkId} by user {UserId}", dto.ArtworkId, userId);
            var createdAuctionDto = _mapper.Map<AuctionDto>(createdAuction);
            return CreatedAtAction(nameof(GetAuctionDetails), new { auctionId = createdAuction.Id }, createdAuctionDto);
        }

        /// <summary>
        /// Updates an existing auction.
        /// </summary>
        /// <param name="auctionId">ID of the auction to update.</param>
        /// <param name="dto">Updated auction data.</param>
        /// <returns>200 OK with the updated auction, 404 NotFound if the auction is missing, or 400 BadRequest if invalid data is provided.</returns>
        [HttpPut("{auctionId}")] // PUT /api/auctions/{auctionId}
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> EditAuction(int auctionId, [FromBody] AuctionDto dto)
        {
            int userId = User.GetUserId();
            _logger.LogInformation("User {UserId} is editing auction {AuctionId}", userId, auctionId);
            var auction = _mapper.Map<Auction>(dto);
            var updatedAuction = await _auctionService.UpdateAuctionAsync(userId, auctionId, auction);
            _logger.LogInformation("Auction {AuctionId} updated by user {UserId}", auctionId, userId);
            var updatedAuctionDto = _mapper.Map<AuctionDto>(updatedAuction);
            return Ok(updatedAuctionDto);
        }

        /// <summary>
        /// Deletes an auction by its ID.
        /// </summary>
        /// <param name="auctionId">ID of the auction to delete.</param>
        /// <returns>204 NoContent if deletion is successful, or 404 NotFound if the auction does not exist.</returns>
        [HttpDelete("{auctionId}")] // DELETE /api/auctions/{auctionId}
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> DeleteAuction(int auctionId)
        {
            int userId = User.GetUserId();
            _logger.LogInformation("User {UserId} is deleting auction {AuctionId}", userId, auctionId);
            await _auctionService.DeleteAuctionAsync(userId, auctionId);
            _logger.LogInformation("Auction {AuctionId} deleted by user {UserId}", auctionId, userId);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all currently active auctions.
        /// </summary>
        /// <returns>200 OK with a list of active auctions.</returns>
        [HttpGet("active")] // GET /api/auctions/active
        public async Task<IActionResult> GetActiveAuctions()
        {
            _logger.LogInformation("Retrieving all active auctions");
            var activeAuctions = await _auctionService.GetActiveAuctionsAsync();
            var activeAuctionDtos = _mapper.Map<IEnumerable<AuctionDto>>(activeAuctions);
            return Ok(activeAuctionDtos);
        }

        /// <summary>
        /// Retrieves all auctions created by the current user.
        /// (For now, returns all auctions without filtering by user.)
        /// </summary>
        /// <returns>200 OK with a list of auctions.</returns>
        [HttpGet("my")] // GET /api/auctions/my
        [Authorize(Roles = RoleNames.Artist)]
        public async Task<IActionResult> GetMyAuctions()
        {
            int userId = User.GetUserId();
            _logger.LogInformation("Retrieving auctions for user {UserId}", userId);
            var myAuctions = await _auctionService.GetUserAuctionsAsync(userId);
            var myAuctionDtos = _mapper.Map<IEnumerable<AuctionDto>>(myAuctions);
            return Ok(myAuctionDtos);
        }

        /// <summary>
        /// Retrieves details of a specific auction by ID.
        /// </summary>
        /// <param name="auctionId">ID of the auction.</param>
        /// <returns>200 OK with auction details, or 404 NotFound if the auction does not exist.</returns>
        [HttpGet("{auctionId}")] // GET /api/auctions/{auctionId}
        public async Task<IActionResult> GetAuctionDetails(int auctionId)
        {
            _logger.LogInformation("Retrieving details for auction {AuctionId}", auctionId);
            var auctionDetails = await _auctionService.GetAuctionByIdAsync(auctionId);
            if (auctionDetails == null)
            {
                _logger.LogWarning("Auction with ID {AuctionId} not found", auctionId);
                return NotFound(new { message = $"Auction with ID {auctionId} not found." });
            }
            var auctionDetailsDto = _mapper.Map<AuctionDto>(auctionDetails);
            return Ok(auctionDetailsDto);
        }
    }
}