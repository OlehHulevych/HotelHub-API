using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.DTO;
using server.models;
using server.Repository;

namespace server.Controllers;
[Route("api/RoomType")]
[ApiController]
public class RoomTypeController:ControllerBase
{
    public ApplicationDbContext _context;
    public RoomTypeRepository _roomTypeRepository;

    public RoomTypeController(ApplicationDbContext context, RoomTypeRepository roomTypeRepository)
    {
        _context = context;
        _roomTypeRepository = roomTypeRepository;
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<IActionResult> createRoomType([FromForm] RoomTypeDTO? data)
    {
        if (string.IsNullOrWhiteSpace(data.Name))
        {
            return BadRequest(new { message = "There is no name of room type" });
        }

        var result = await _roomTypeRepository.AddRoomType(data);
        if (!result.result)
        {
            return BadRequest(new {message = "Something went wrong"});
        }

        return Ok(result);

    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost("update/{id}")]
    public async Task<IActionResult> UpdateRoomType([FromForm] UpdateRoomTypeDTO data, Guid id)
    {
        var response = await _roomTypeRepository.UpdateRoomType(data,id);
        if (!response.result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);

    }
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeRoom(Guid id)
    {
        if (id == null)
        {
            return BadRequest(new {message = "There is no id of room type"});
        }

        var response = await _roomTypeRepository.RemoveRoomType(id);
        if (!response.result)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }
}