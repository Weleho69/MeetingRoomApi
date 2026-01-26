using MeetingRoomApi.Data;
using MeetingRoomApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace MeetingRoomApi.Helpers;

public static class ReservationValidator
{
    public static async Task ValidateAsync(AppDbContext context, Reservation reservation)
    {
        if (reservation.StartUtc >= reservation.EndUtc)
            throw new ArgumentException("Start date must be before end date.");

        if (reservation.StartUtc < DateTime.UtcNow)
            throw new ArgumentException("Reservations cannot be made in the past.");

        var overlaps = await context.Reservations.AnyAsync(r =>
            r.MeetingRoomId == reservation.MeetingRoomId &&
            r.Id != reservation.Id &&
            reservation.StartUtc < r.EndUtc &&
            reservation.EndUtc > r.StartUtc
        );

        if (overlaps)
            throw new ArgumentException("Reservation overlaps with an existing reservation.");
    }
}