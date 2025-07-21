using ArtAuctionHub.Application.DTOs.ArtWork;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The ArtworksController manages CRUD operations and queries related to artworks.
    /// </summary>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/artworks")] // Base route for all artwork-related endpoints. (/api/artworks)
    public class ArtworksController : ControllerBase
    {
        /// <summary>
        /// Creates a new artwork based on the provided data.
        /// </summary>
        /// <param name="dto">
        /// An ArtworkDto object containing the artwork's details (title, description, category, etc.).
        /// </param>
        /// <returns>
        /// A 201 Created response with the created artwork data.
        /// </returns>
        [HttpPost] // Matches POST /api/artworks.
        public IActionResult CreateArtwork([FromBody] ArtworkDto dto)
        {
            // Future implementation would involve:
            // 1. Validate the dto.
            // 2. Save the artwork to the database.
            // 3. Return the created resource (possibly with its new ID).
            return Created("", dto); // Placeholder response: returns the same dto.
        }

        /// <summary>
        /// Updates an existing artwork identified by its ID.
        /// </summary>
        /// <param name="id">The ID of the artwork to update.</param>
        /// <param name="dto">An ArtworkDto object containing updated data.</param>
        /// <returns>
        /// A 200 OK response with the updated artwork data.
        /// </returns>
        [HttpPut]
        [Route("{id}")] // Matches PUT /api/artworks/5.
        public IActionResult UpdateArtwork(int id, [FromBody] ArtworkDto dto)
        {
            // Future implementation would involve:
            // 1. Check if the artwork with the given ID exists.
            // 2. Update its properties based on the provided dto.
            return Ok(dto); // Placeholder response.
        }

        /// <summary>
        /// Deletes an artwork by its ID.
        /// </summary>
        /// <param name="id">The ID of the artwork to delete.</param>
        /// <returns>
        /// A 204 No Content response if deletion is successful.
        /// </returns>
        [HttpDelete]
        [Route("{id}")] // Matches DELETE /api/artworks/5.
        public IActionResult DeleteArtwork(int id)
        {
            // Future implementation would involve:
            // 1. Check if the artwork exists.
            // 2. Delete it from the database.
            return NoContent(); // Returns 204 No Content.
        }

        /// <summary>
        /// Retrieves all artworks.
        /// </summary>
        /// <returns>
        /// A 200 OK response with a list of artworks.
        /// </returns>
        [HttpGet] // Matches GET /api/artworks.
        public IActionResult GetAll()
        {
            // Future implementation would involve fetching artworks from the database.
            return Ok(new[] { "Artwork1", "Artwork2" }); // Example data.
        }

        /// <summary>
        /// Retrieves all artworks created by the currently authenticated user.
        /// </summary>
        /// <returns>
        /// A 200 OK response with the user's artworks.
        /// </returns>
        [HttpGet]
        [Route("my")] // Matches GET /api/artworks/my.
        public IActionResult GetMyArtworks()
        {
            // Future implementation would involve fetching artworks from the database based on the authenticated user.
            return Ok(new[] { "MyArtwork1" }); // Placeholder data.
        }
    }
}