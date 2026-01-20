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
    public ApplicationDbContext Context;
    public RoomTypeRepository RoomTypeRepository;

    public RoomTypeController(ApplicationDbContext context, RoomTypeRepository roomTypeRepository)
    {
        Context = context;
        RoomTypeRepository = roomTypeRepository;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetTypes([FromQuery]Guid id)
    {
        var response = await RoomTypeRepository.GetRoomTypes(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<IActionResult> CreateRoomType([FromForm] RoomTypeDto? data)
    {
        if (string.IsNullOrWhiteSpace(data.Name))
        {
            return BadRequest(new { message = "There is no name of room type" });
        }

        var result = await RoomTypeRepository.AddRoomType(data);
        if (!result.Result)
        {
            return BadRequest(new {message = "Something went wrong"});
        }

        return Ok(result);

    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost("update/{id}")]
    public async Task<IActionResult> UpdateRoomType([FromForm] UpdateRoomTypeDto data, Guid id)
    {
        var response = await RoomTypeRepository.UpdateRoomType(data,id);
        if (!response.Result)
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

        var response = await RoomTypeRepository.RemoveRoomType(id);
        if (!response.Result)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }
}