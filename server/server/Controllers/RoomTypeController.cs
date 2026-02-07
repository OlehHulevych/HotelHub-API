using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Repository;

namespace server.Controllers;


[Route("api/RoomType")]
[ApiController]
public class RoomTypeController : ControllerBase
{
    
    private readonly RoomTypeRepository _roomTypeRepository;

    public RoomTypeController( RoomTypeRepository roomTypeRepository)
    {
        
        _roomTypeRepository = roomTypeRepository;
    }

    
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

    
    [Authorize(Roles = "ADMIN,OWNER")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTypeRoom(Guid id)
    {
        
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
