using MeetingRoomApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomApi.Controllers;

[ApiController]
[Route("api/meeting-rooms")]
public class MeetingRoomsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MeetingRoomsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _context.MeetingRooms.ToListAsync();
        return Ok(rooms);
    }
}