using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Repository;

namespace server.Controllers;

[Route("api/room")]
[ApiController]
public class RoomController:ControllerBase
{
    private RoomRepository _roomRepository;

    public RoomController(RoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> getAll([FromQuery] PaginationDTO queries)
    {
       
        var response = await _roomRepository.getALlRooms(queries);
        if (response.Items==null)
        {
            return BadRequest(new { message = "Something went wrong" });
        }

        return Ok(response);
    }
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<IActionResult> createRoom([FromForm] RoomDTO data)
    {
        if (data == null)
        {
            return BadRequest(new { message = "There is no data" });
        }

        var response = await _roomRepository.createRoom(data);
        if (!response.result)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }

   
    
    [Authorize(Roles = "ADMIN")]
    [HttpDelete]
    public async Task<IActionResult> deleteRoom([FromQuery] Guid id)
    {
        if (id == null)
        {
            return BadRequest("There is no id");
        }

        var response = await _roomRepository.deleteRoom(id);
        if (!response.result)
        {
            return BadRequest(response.Message);
        }
        else
        {
            return Ok(response);
        }
    }
}