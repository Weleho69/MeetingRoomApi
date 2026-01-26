using System;

namespace MeetingRoomApi.Models;

public class MeetingRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}