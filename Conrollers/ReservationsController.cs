using MeetingRoomApi.Data;
using MeetingRoomApi.Helpers;
using MeetingRoomApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace MeetingRoomApi.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReservationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Reservations
                                .Include(r => r.MeetingRoom)
                                .Select(r => new Reservation 
                                { 
                                    Id = r.Id,
                                    MeetingRoomId = r.MeetingRoomId,
                                    StartUtc = r.StartUtc,
                                    EndUtc = r.EndUtc,
                                    ReservedBy = r.ReservedBy,
                                    MeetingRoom = _context.MeetingRooms.Where(m => m.Id == r.MeetingRoomId).FirstOrDefault() 
                                }).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        return reservation is null ? NotFound() : Ok(reservation);
    }

    [HttpGet("by-room/{roomId:guid}")]
    public async Task<IActionResult> GetByRoom(Guid roomId)
    {
        var roomExists = await _context.MeetingRooms.AnyAsync(r => r.Id == roomId);
        if (!roomExists)
            return NotFound("Meeting room not found.");

        var reservations = await _context.Reservations
            .Where(r => r.MeetingRoomId == roomId)
            .OrderBy(r => r.StartUtc).Include(r => r.MeetingRoom)
            .Select(r => new Reservation
            {
                Id = r.Id,
                MeetingRoomId = r.MeetingRoomId,
                StartUtc = r.StartUtc,
                EndUtc = r.EndUtc,
                ReservedBy = r.ReservedBy,
                MeetingRoom = _context.MeetingRooms.Where(m => m.Id == r.MeetingRoomId).FirstOrDefault()
            })
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Reservation reservation)
    {
        //Added expanded error handling via transactions in case of interruption 
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await ReservationValidator.ValidateAsync(_context, reservation);

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction(nameof(Get), new { id = reservation.Id }, reservation);
        }
        catch (ArgumentException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Reservation updated)
    {
        //Added expanded error handling via transactions in case of interruption 
        await using var transaction = await _context.Database.BeginTransactionAsync();
        if (id != updated.Id)
            updated.Id = id;

        var exists = await _context.Reservations.AnyAsync(r => r.Id == id);
        if (!exists)
            return NotFound();

        try
        {
            await ReservationValidator.ValidateAsync(_context, updated);

            _context.Reservations.Update(updated);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        //Added expanded error handling via transactions in case of interruption 
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation is null)
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return NoContent();
        } catch (ArgumentException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }
}
