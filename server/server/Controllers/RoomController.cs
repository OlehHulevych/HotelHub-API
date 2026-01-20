using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Repository;

namespace server.Controllers;

/// <summary>
/// Room endpoints.
/// </summary>
/// <remarks>
/// Provides operations for listing rooms and (admin-only) creating/deleting rooms.
/// </remarks>
[Route("api/room")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly RoomRepository _roomRepository;

    public RoomController(RoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    /// <summary>
    /// Get all rooms (paginated).
    /// </summary>
    /// <param name="queries">Pagination parameters (page, pageSize, etc.).</param>
    /// <returns>A paginated list of rooms.</returns>
    /// <response code="200">Rooms returned successfully.</response>
    /// <response code="400">Request failed (e.g., repository returned no items).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationDto queries)
    {
        var response = await _roomRepository.GetALlRooms(queries);

        if (!response.Items.Any())
        {
            return BadRequest(new { message = "Something went wrong" });
        }

        return Ok(response);
    }

    /// <summary>
    /// Create a new room (admin only).
    /// </summary>
    /// <param name="data">
    /// Room payload sent as multipart/form-data (because it uses <c>[FromForm]</c>).
    /// </param>
    /// <returns>Result of the create operation.</returns>
    /// <response code="200">Room created successfully.</response>
    /// <response code="400">Validation error / missing data / create failed.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRoom([FromForm] RoomDto? data)
    {
        if (data == null)
        {
            return BadRequest(new { message = "There is no data" });
        }

        var response = await _roomRepository.CreateRoom(data);
        if (!response.Result)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }

    /// <summary>
    /// Delete a room by id (admin only).
    /// </summary>
    /// <param name="id">Room id (query parameter).</param>
    /// <returns>Result of the delete operation.</returns>
    /// <response code="200">Room deleted successfully.</response>
    /// <response code="400">Delete failed (invalid id / not found / etc.).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteRoom([FromQuery] Guid id)
    {
        // Note: Guid is a non-nullable value type, so "id == null" is always false.
        // If you want to validate it, check Guid.Empty.
        if (id == Guid.Empty)
        {
            return BadRequest("There is no id");
        }

        var response = await _roomRepository.DeleteRoom(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
}
