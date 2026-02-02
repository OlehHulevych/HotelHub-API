using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Repository;

namespace server.Controllers;


[Route("api/room")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly RoomRepository _roomRepository;

    public RoomController(RoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    
    [HttpGet]
    
    public async Task<IActionResult> GetAll([FromQuery] PaginationDto queries)
    {
        var response = await _roomRepository.GetALlRooms(queries);

        if (!response.Items.Any())
        {
            return BadRequest(new { message = "Something went wrong" });
        }

        return Ok(response);
    }
    [Authorize(Roles = "ADMIN, OWNER")]
    [HttpPatch]
    public async Task<IActionResult> UpdateRoom([FromQuery] Guid id)
    {
        ResultDto response = await _roomRepository.PutInMaintenance(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    
    [Authorize(Roles = "ADMIN, OWNER")]
    [HttpPost]
    
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
    

   
    [Authorize(Roles = "ADMIN,OWNER")]
    [HttpDelete]
    
    public async Task<IActionResult> DeleteRoom([FromQuery] Guid id)
    {
        
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
