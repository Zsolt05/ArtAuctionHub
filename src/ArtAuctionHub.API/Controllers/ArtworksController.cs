using ArtAuctionHub.Application.DTOs.ArtWork;
using ArtAuctionHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The ArtworksController manages CRUD operations and queries related to artworks.
    /// </summary>
    /// <param name="artworkService">An instance of IArtworkService to handle artwork operations. Registered in the Dependency Injection (DI) container.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/artworks")] // Base route for all artwork-related endpoints. (/api/artworks)
    public class ArtworksController(IArtworkService artworkService) : ControllerBase
    {
        /// <summary>
        /// Creates a new artwork and uploads an image.
        /// </summary>
        /// <param name="dto">The artwork data.</param>
        /// <param name="imageFile">The image file to be uploaded.</param>
        /// <returns>Returns the created artwork data.</returns>
        [HttpPost]
        // POST /api/artworks
        public async Task<IActionResult> CreateArtworkAsync([FromForm] ArtworkDto dto, [FromForm] IFormFile imageFile)
        {
            var createdArtwork = await artworkService.CreateArtworkAsync(dto, imageFile);
            return CreatedAtAction(nameof(GetAll), new { id = createdArtwork.Id }, createdArtwork);
        }

        /// <summary>
        /// Updates an existing artwork (excluding image).
        /// </summary>
        [HttpPut("{id}")]
        // PUT /api/artworks/{id}
        public async Task<IActionResult> UpdateArtworkAsync(int id, [FromBody] ArtworkDto dto)
        {
            var updatedArtwork = await artworkService.UpdateArtworkAsync(id, dto);
            return Ok(updatedArtwork);
        }

        /// <summary>
        /// Deletes an artwork and its image.
        /// </summary>
        [HttpDelete("{id}")]
        // DELETE /api/artworks/{id}
        public async Task<IActionResult> DeleteArtworkAsync(int id)
        {
            await artworkService.DeleteArtworkAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all artworks from the database.
        /// </summary>
        [HttpGet]
        // GET /api/artworks
        public async Task<IActionResult> GetAll()
        {
            var artworks = await artworkService.GetAllArtworksAsync();
            return Ok(artworks);
        }

        /// <summary>
        /// Retrieves artworks created by the currently authenticated user.
        /// </summary>
        [HttpGet("my/{userId}")]
        // GET /api/artworks/my/{userId}
        public async Task<IActionResult> GetMyArtworksAsync(int userId)
        {
            var artworks = await artworkService.GetMyArtworksAsync(userId);
            return Ok(artworks);
        }
    }
}