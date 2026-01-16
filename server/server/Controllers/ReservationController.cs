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
    public async Task<IActionResult> GetAllReservation([FromQuery] PaginationDTO query)
    {
        var response = await _reservationRepository.getAllReservation(query);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservationById(Guid id)
    {
        var response = await _reservationRepository.getOneReservation(id);
        if (!response.result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetAllReservationById([FromQuery] PaginationDTO query)
    {
        string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var response = await _reservationRepository.getAllReservationById(query, id);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PostReservation([FromForm] ReservationDTO? data)
    {
        if (data == null)
        {
            return BadRequest("There is no data for reservation");
        }

        string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id != null)
        {
            var response = await _reservationRepository.createReservation(data, id);
            if (!response.result)
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
    public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationDTO? data, [FromQuery] Guid id )
    {
        if (data == null)
        {
            return BadRequest("There is not data for editing");
        }

        var response = await _reservationRepository.editReservation(data, id);
        if (!response.result)
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
        var response = await _reservationRepository.deleteReservation(id, userId);
        if (!response.result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    

}