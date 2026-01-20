using System.Security.Claims;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Repository;

namespace server.Controllers;
[Route("api/reservation")]
public class ReservationController:ControllerBase
{
    private readonly ReservationRepository _reservationRepository;

    public ReservationController(ReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public async Task<IActionResult> GetAllReservation([FromQuery] PaginationDto query)
    {
        var response = await _reservationRepository.GetAllReservation(query);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservationById(Guid id)
    {
        var response = await _reservationRepository.getOneReservation(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetAllReservationById([FromQuery] PaginationDto query)
    {
        string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var response = await _reservationRepository.GetAllReservationById(query, id);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
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
        else
        {
            return BadRequest("There is no user id");
        }
    }
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationDto? data, [FromQuery] Guid id )
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
    
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteReservation([FromQuery] Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var response = await _reservationRepository.DeleteReservation(id, userId);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    

}