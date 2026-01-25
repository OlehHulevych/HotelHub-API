using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Repository;

namespace server.Controllers;

/// <summary>
/// Reservation endpoints.
/// </summary>
/// <remarks>
/// Provides CRUD operations for reservations.
/// Some endpoints are restricted to ADMIN users.
/// </remarks>
[ApiController]
[Route("api/reservation")]
public class ReservationController : ControllerBase
{
    private readonly ReservationRepository _reservationRepository;

    public ReservationController(ReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    /// <summary>
    /// Get all reservations (admin only).
    /// </summary>
    /// <param name="query">Pagination parameters (page, pageSize, etc.).</param>
    /// <returns>A paginated list of reservations.</returns>
    /// <response code="200">Reservations returned successfully.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN,OWNER")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllReservation([FromQuery] PaginationDto query)
    {
        var response = await _reservationRepository.GetAllReservation(query);
        return Ok(response);
    }

    /// <summary>
    /// Get a reservation by its id.
    /// </summary>
    /// <param name="id">Reservation id.</param>
    /// <returns>The reservation if found.</returns>
    /// <response code="200">Reservation returned successfully.</response>
    /// <response code="400">Reservation not found or request invalid.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetReservationById(Guid id)
    {
        var response = await _reservationRepository.getOneReservation(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get reservations for the currently logged-in user.
    /// </summary>
    /// <param name="query">Pagination parameters (page, pageSize, etc.).</param>
    /// <returns>A paginated list of the user's reservations.</returns>
    /// <response code="200">Reservations returned successfully.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize]
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllReservationById([FromQuery] PaginationDto query)
    {
        string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // If this is ever null, it means your JWT doesn't include NameIdentifier/sub claim mapping.
        if (id == null)
        {
            return Unauthorized("The user is not authorized");
        }
        var response = await _reservationRepository.GetAllReservationById(query, id);
        return Ok(response);
    }

    /// <summary>
    /// Create a reservation for the currently logged-in user.
    /// </summary>
    /// <param name="data">
    /// Reservation payload sent as multipart/form-data (because it uses <c>[FromForm]</c>).
    /// </param>
    /// <returns>Result of the create operation.</returns>
    /// <response code="200">Reservation created successfully.</response>
    /// <response code="400">Validation error / missing data / user id missing.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostReservation([FromForm] ReservationDto? data)
    {
        if (data == null)
        {
            return BadRequest("There is no data for reservation");
        }

        string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id != null)
        {
            var response = await _reservationRepository.CreateReservation(data, id);
            if (!response.Result)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        return BadRequest("There is no user id");
    }

    /// <summary>
    /// Update a reservation.
    /// </summary>
    /// <param name="id">Reservation id (query parameter).</param>
    /// <param name="data">Fields to update.</param>
    /// <returns>Result of the update operation.</returns>
    /// <response code="200">Reservation updated successfully.</response>
    /// <response code="400">Validation error / reservation not found.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationDto? data, [FromQuery] Guid id)
    {
        if (data == null)
        {
            return BadRequest("There is not data for editing");
        }

        var response = await _reservationRepository.EditReservation(data, id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    /// <summary>
    /// Delete a reservation.
    /// </summary>
    /// <param name="id">Reservation id (query parameter).</param>
    /// <returns>Result of the delete operation.</returns>
    /// <response code="200">Reservation deleted successfully.</response>
    /// <response code="400">Delete failed (not found / not allowed / etc.).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteReservation([FromQuery] Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
           return Unauthorized("The user is no authorized");
        }

        var response = await _reservationRepository.DeleteReservation(id, userId);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
}
