using ArtAuctionHub.Application.DTOs.Bid;
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
    /// The BidsController handles placing and retrieving bids on auctions.
    /// </summary>
    /// <remarks>
    /// Constructor for BidsController.
    /// </remarks>
    /// <param name="bidService">An instance of IBidService to handle bid operations. Registered in the Dependency Injection (DI) container.</param>
    /// <param name="logger">The logger instance for logging bid actions.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/bids")] // Base route for all bid-related endpoints. (/api/bids/place)
    [Authorize]
    public class BidsController : ControllerBase
    {
        private readonly IBidService _bidService;
        private readonly ILogger<BidsController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for BidsController.
        /// </summary>
        /// <param name="bidService">Injected bid service.</param>
        /// <param name="logger">Logger for bid actions.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public BidsController(IBidService bidService,
            ILogger<BidsController> logger, IMapper mapper)
        {
            _bidService = bidService;
            _logger = logger;
            _mapper = mapper;
        }

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
            _logger.LogInformation("User {UserId} is placing a bid on auction {AuctionId}", userId, dto.AuctionId);
            var createBid = _mapper.Map<Bid>(dto);
            await _bidService.PlaceBidAsync(createBid, userId);
            _logger.LogInformation("Bid placed successfully by user {UserId} on auction {AuctionId}", userId, dto.AuctionId);
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
            _logger.LogInformation("Retrieving bids for user {UserId}", userId);
            var myBids = await _bidService.GetBidsForUserAsync(userId);
            var myBidsDto = _mapper.Map<List<ReadBidDto>>(myBids);
            return Ok(myBidsDto); // Returns 200 OK with the user's bids.
        }
    }
}