using System;

namespace MeetingRoomApi.Models;

public class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MeetingRoomId { get; set; }
    public MeetingRoom? MeetingRoom { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public string ReservedBy { get; set; } = string.Empty;
}