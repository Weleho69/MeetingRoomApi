using MeetingRoomApi.Data;
using MeetingRoomApi.Models;


namespace MeetingRoomApi.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.MeetingRooms.Any())
            return;

        context.MeetingRooms.AddRange(
            new MeetingRoom { Name = "Orion", Capacity = 4 },
            new MeetingRoom { Name = "Atlas", Capacity = 6 },
            new MeetingRoom { Name = "Apollo", Capacity = 8 },
            new MeetingRoom { Name = "Nova", Capacity = 10 },
            new MeetingRoom { Name = "Zenith", Capacity = 12 }
        );

        context.SaveChanges();
    }
}
