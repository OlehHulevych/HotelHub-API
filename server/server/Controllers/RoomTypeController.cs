using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Repository;

namespace server.Controllers;

/// <summary>
/// Room type endpoints.
/// </summary>
/// <remarks>
/// Provides operations for reading room types and (admin-only) creating/updating/deleting room types.
/// </remarks>
[Route("api/RoomType")]
[ApiController]
public class RoomTypeController : ControllerBase
{
    
    private readonly RoomTypeRepository _roomTypeRepository;

    public RoomTypeController( RoomTypeRepository roomTypeRepository)
    {
        
        _roomTypeRepository = roomTypeRepository;
    }

    /// <summary>
    /// Get room types (optionally filtered by an id).
    /// </summary>
    /// <param name="id">
    /// Optional filter id passed as a query parameter.
    /// If you don't want filtering, pass <c>Guid.Empty</c> (or consider making this nullable).
    /// </param>
    /// <returns>A list of room types.</returns>
    /// <response code="200">Room types returned successfully.</response>
    /// <response code="400">Request failed (repository returned an error).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTypes([FromQuery] Guid id)
    {
        var response = await _roomTypeRepository.GetRoomTypes(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    /// <summary>
    /// Create a new room type (admin only).
    /// </summary>
    /// <param name="data">
    /// Room type payload sent as multipart/form-data (because it uses <c>[FromForm]</c>).
    /// </param>
    /// <returns>Result of the create operation.</returns>
    /// <response code="200">Room type created successfully.</response>
    /// <response code="400">Validation error / create failed.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN,OWNER")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRoomType([FromForm] RoomTypeDto? data)
    {
        if (data == null || string.IsNullOrWhiteSpace(data.Name))
        {
            return BadRequest(new { message = "There is no name of room type" });
        }

        var result = await _roomTypeRepository.AddRoomType(data);
        if (!result.Result)
        {
            return BadRequest(new { message = "Something went wrong" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Update an existing room type (admin only).
    /// </summary>
    /// <param name="id">Room type id (route parameter).</param>
    /// <param name="data">Fields to update, sent as multipart/form-data.</param>
    /// <returns>Result of the update operation.</returns>
    /// <response code="200">Room type updated successfully.</response>
    /// <response code="400">Update failed (invalid id / not found / validation error).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN,OWNER")]
    [HttpPost("update/{id:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRoomType([FromForm] UpdateRoomTypeDto data, Guid id)
    {
        var response = await _roomTypeRepository.UpdateRoomType(data, id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    /// <summary>
    /// Delete a room type by id (admin only).
    /// </summary>
    /// <param name="id">Room type id (route parameter).</param>
    /// <returns>Result of the delete operation.</returns>
    /// <response code="200">Room type deleted successfully.</response>
    /// <response code="400">Delete failed (invalid id / not found / etc.).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN,OWNER")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTypeRoom(Guid id)
    {
        // Guid is non-nullable, so check Guid.Empty instead of null.
        if (id == Guid.Empty)
        {
            return BadRequest(new { message = "There is no id of room type" });
        }

        var response = await _roomTypeRepository.RemoveRoomType(id);
        if (!response.Result)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }
}
