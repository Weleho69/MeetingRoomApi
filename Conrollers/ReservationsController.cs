using MeetingRoomApi.Data;
using MeetingRoomApi.Helpers;
using MeetingRoomApi.Models;
using MeetingRoomAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                                .Include(r => r.Customer)
                                .OrderBy(r => r.StartUtc)
                                .Select(r => new ReservationWithCustomer
                                {
                                    Id = r.Id,
                                    StartUtc = r.StartUtc,
                                    EndUtc = r.EndUtc,
                                    Customer = new CustomerInfo
                                    {
                                        email = r.Customer!.Email,
                                        phone = r.Customer.Phone,
                                        name = r.Customer.Name
                                    },
                                    Room = new RoomInfo
                                    {
                                        id = r.MeetingRoom!.Id,
                                        name = r.MeetingRoom.Name,
                                        capacity = r.MeetingRoom.Capacity

                                    }
                                }).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var reservation = await _context.Reservations
                                .Include(r => r.MeetingRoom)
                                .Include(r => r.Customer)
                                .Select(r => new ReservationWithCustomer
        {
            Id = r.Id,
            StartUtc = r.StartUtc,
            EndUtc = r.EndUtc,
            Customer = new CustomerInfo
            {
                email = r.Customer!.Email,
                phone = r.Customer.Phone,
                name = r.Customer.Name
            },
            Room = new RoomInfo
            {
                id = r.MeetingRoom!.Id,
                name = r.MeetingRoom.Name,
                capacity = r.MeetingRoom.Capacity

            }
        }).FirstOrDefaultAsync();
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
            .OrderBy(r => r.StartUtc)
            .Include(r => r.MeetingRoom)
            .Include(r => r.Customer)
            .Select(r => new ReservationWithCustomer
            {
                Id = r.Id,
                StartUtc = r.StartUtc,
                EndUtc = r.EndUtc,
                Customer = new CustomerInfo
                {
                    email = r.Customer!.Email,
                    phone = r.Customer.Phone,
                    name = r.Customer.Name
                },
                Room = new RoomInfo
                {
                    id = r.MeetingRoom!.Id,
                    name = r.MeetingRoom.Name,
                    capacity = r.MeetingRoom.Capacity

                }
            })
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservationResponse reservationResponse)
    {
        //Added expanded error handling via transactions in case of interruption 
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var customer = await _context.Customers.Where(c => c.Email == reservationResponse.CustomerEmail).FirstOrDefaultAsync();
            if (customer == null)
            {
                return NotFound("No user found with email");
            }

            Reservation newReservation = new Reservation
            {
                MeetingRoomId = reservationResponse.MeetingRoomId,
                EndUtc = reservationResponse.EndUtc,
                StartUtc = reservationResponse.StartUtc,
                CustomerId = customer.Id
            };
            await ReservationValidator.ValidateAsync(_context, newReservation);
            _context.Reservations.Add(newReservation);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction(nameof(Get), new { id = newReservation.Id }, newReservation);
        }
        catch (ArgumentException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, ReservationResponse updated)
    {
        //Added expanded error handling via transactions in case of interruption 
        await using var transaction = await _context.Database.BeginTransactionAsync();
        if (id != updated.Id)
            updated.Id = id;

        var exists = await _context.Reservations.AnyAsync(r => r.Id == id);
        if (!exists)
            return NotFound();

        var customer = await _context.Customers.Where(c => c.Email == updated.CustomerEmail).FirstOrDefaultAsync();
        if (customer == null)
        {
            return NotFound("No user found with email");
        }

        try
        {

            Reservation newReservation = new Reservation
            {
                MeetingRoomId = updated.MeetingRoomId,
                EndUtc = updated.EndUtc,
                StartUtc = updated.StartUtc,
                CustomerId = customer.Id
            };
            await ReservationValidator.ValidateAsync(_context, newReservation);
            

            _context.Reservations.Update(newReservation);
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
