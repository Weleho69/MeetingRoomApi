## Työssä käytetty ilmaisversiota ChatGPT versio 5.2, www.chatgpt.com

Generate a meeting room reservation api. 
Use C# .NET Api with dotnet core 8. Create 5 meeting rooms, list their capacity and generate and list unique names for them. 
implement basic CRUD functions, check that dates are not overlapping per room, can't be reserved in the past and start date is before the end date.

 
> Nice, this is a clean, classic API problem. Below is a minimal but production-sane ASP.NET Core 8 Web API that:
> 
> Creates 5 meeting rooms with unique names + capacities
> 
> Exposes CRUD endpoints for reservations
> 
> Validates:
> 
> ❌ No overlapping reservations per room
> 
> ❌ No reservations in the past
> 
> ❌ Start date must be before end date
> 
> I’ll keep it in-memory for simplicity (easy to swap for EF Core later).
> 
> 1. Create the project
> dotnet new webapi -n MeetingRoomReservationApi
> cd MeetingRoomReservationApi
> 
> 
> Target framework in .csproj should be:
> 
> <TargetFramework>net8.0</TargetFramework>
> 
> 2. Models
> Models/MeetingRoom.cs
>  > namespace MeetingRoomReservationApi.Models;
>  > 
>  > public class MeetingRoom
>  > {
>  >     public Guid Id { get; set; } = Guid.NewGuid();
>  >     public string Name { get; set; } = string.Empty;
>  >     public int Capacity { get; set; }
>  > }
> 
> Models/Reservation.cs
>  > namespace MeetingRoomReservationApi.Models;
>  > 
>  > public class Reservation
>  > {
>  >     public Guid Id { get; set; } = Guid.NewGuid();
>  >     public Guid MeetingRoomId { get; set; }
>  >     public DateTime StartUtc { get; set; }
>  >     public DateTime EndUtc { get; set; }
>  >     public string ReservedBy { get; set; } = string.Empty;
>  > }
> 
> 3. In-Memory Data Store
> Data/InMemoryStore.cs
>  > using MeetingRoomReservationApi.Models;
>  > 
>  > namespace MeetingRoomReservationApi.Data;
>  > 
>  > public static class InMemoryStore
>  > {
>  >     public static List<MeetingRoom> MeetingRooms { get; } =
>  >     [
>  >         new() { Name = "Orion", Capacity = 4 },
>  >         new() { Name = "Atlas", Capacity = 6 },
>  >         new() { Name = "Apollo", Capacity = 8 },
>  >         new() { Name = "Nova", Capacity = 10 },
>  >         new() { Name = "Zenith", Capacity = 12 }
>  >     ];
>  > 
>  >     public static List<Reservation> Reservations { get; } = [];
>  > }
> 
> 4. Validation Helper
> Helpers/ReservationValidator.cs
>  > using MeetingRoomReservationApi.Data;
>  > using MeetingRoomReservationApi.Models;
>  > 
>  > namespace MeetingRoomReservationApi.Helpers;
>  > 
>  > public static class ReservationValidator
>  > {
>  >     public static void Validate(Reservation reservation)
>  >     {
>  >         if (reservation.StartUtc >= reservation.EndUtc)
>  >             throw new ArgumentException("Start date must be before end date.");
>  > 
>  >         if (reservation.StartUtc < DateTime.UtcNow)
>  >             throw new ArgumentException("Reservations cannot be made in the past.");
>  > 
>  >         var overlaps = InMemoryStore.Reservations.Any(r =>
>  >             r.MeetingRoomId == reservation.MeetingRoomId &&
>  >             r.Id != reservation.Id &&
>  >             reservation.StartUtc < r.EndUtc &&
>  >             reservation.EndUtc > r.StartUtc
>  >         );
>  > 
>  >         if (overlaps)
>  >             throw new ArgumentException("Reservation overlaps with an existing reservation.");
>  >     }
>  > }
> 
> 5. Controllers
> Controllers/MeetingRoomsController.cs
> > using MeetingRoomReservationApi.Data;
> > using Microsoft.AspNetCore.Mvc;
> > 
> > namespace MeetingRoomReservationApi.Controllers;
> > 
> > [ApiController]
> > [Route("api/meeting-rooms")]
> > public class MeetingRoomsController : ControllerBase
> > {
> >     [HttpGet]
> >     public IActionResult GetAll()
> >     {
> >         return Ok(InMemoryStore.MeetingRooms);
> >     }
> > }
> > 
> > Controllers/ReservationsController.cs
> > using MeetingRoomReservationApi.Data;
> > using MeetingRoomReservationApi.Helpers;
> > using MeetingRoomReservationApi.Models;
> > using Microsoft.AspNetCore.Mvc;
> > 
> > namespace MeetingRoomReservationApi.Controllers;
> > 
> > [ApiController]
> > [Route("api/reservations")]
> > public class ReservationsController : ControllerBase
> > {
> >     [HttpGet]
> >     public IActionResult GetAll()
> >     {
> >         return Ok(InMemoryStore.Reservations);
> >     }
> > 
> >     [HttpGet("{id:guid}")]
> >     public IActionResult Get(Guid id)
> >     {
> >         var reservation = InMemoryStore.Reservations.FirstOrDefault(r => r.Id == id);
> >         return reservation is null ? NotFound() : Ok(reservation);
> >     }
> > 
> >     [HttpPost]
> >     public IActionResult Create(Reservation reservation)
> >     {
> >         try
> >         {
> >             ReservationValidator.Validate(reservation);
> >             InMemoryStore.Reservations.Add(reservation);
> >             return CreatedAtAction(nameof(Get), new { id = reservation.Id }, reservation);
> >         }
> >         catch (ArgumentException ex)
> >         {
> >             return BadRequest(ex.Message);
> >         }
> >     }
> > 
> >     [HttpPut("{id:guid}")]
> >     public IActionResult Update(Guid id, Reservation updated)
> >     {
> >         var existing = InMemoryStore.Reservations.FirstOrDefault(r => r.Id == id);
> >         if (existing is null) return NotFound();
> > 
> >         updated.Id = id;
> > 
> >         try
> >         {
> >             ReservationValidator.Validate(updated);
> > 
> >             existing.MeetingRoomId = updated.MeetingRoomId;
> >             existing.StartUtc = updated.StartUtc;
> >             existing.EndUtc = updated.EndUtc;
> >             existing.ReservedBy = updated.ReservedBy;
> > 
> >             return NoContent();
> >         }
> >         catch (ArgumentException ex)
> >         {
> >             return BadRequest(ex.Message);
> >         }
> >     }
> > 
> >     [HttpDelete("{id:guid}")]
> >     public IActionResult Delete(Guid id)
> >     {
> >         var reservation = InMemoryStore.Reservations.FirstOrDefault(r => r.Id == id);
> >         if (reservation is null) return NotFound();
> > 
> >         InMemoryStore.Reservations.Remove(reservation);
> >         return NoContent();
> >     }
> > }
> 
> 6. Program.cs
> 
> Nothing fancy needed—default is fine:
> 
> > var builder = WebApplication.CreateBuilder(args);
> > 
> > builder.Services.AddControllers();
> > builder.Services.AddEndpointsApiExplorer();
> > builder.Services.AddSwaggerGen();
> > 
> > var app = builder.Build();
> > 
> > app.UseSwagger();
> > app.UseSwaggerUI();
> > 
> > app.UseHttpsRedirection();
> > app.MapControllers();
> > 
> > app.Run();
> 
> 7. Example API Usage
> List meeting rooms
> GET /api/meeting-rooms
> 
> Create reservation
> POST /api/reservations
> {
>   "meetingRoomId": "ROOM_GUID_HERE",
>   "startUtc": "2026-02-01T10:00:00Z",
>   "endUtc": "2026-02-01T11:00:00Z",
>   "reservedBy": "Alice"
> }
> 
> What you could add next (if you want)
> 
> EF Core + SQL Server
> 
> DTOs instead of raw models
> 
> Time zone handling
> 
> Availability search endpoint
> 
> Room capacity filtering
> 
> If you want, I can:
> 
> Convert this to Minimal APIs
> 
> Add EF Core + migrations
> 
> Add availability endpoint
> 
> Add unit tests
> 
> Just say the word 👌

Add reservation listing by room

> Easy add 👍 — we’ll expose a filter endpoint that lists reservations for a specific meeting room (optionally sorted by start time).
> 
> Below is the cleanest, REST-friendly approach that plugs straight into what you already have.
> 
> 1. Add endpoint to ReservationsController
> Controllers/ReservationsController.cs
> 
> Add this new GET endpoint:
> 
> > [HttpGet("by-room/{roomId:guid}")]
> > public IActionResult GetByRoom(Guid roomId)
> > {
> >     var roomExists = InMemoryStore.MeetingRooms.Any(r => r.Id == roomId);
> >     if (!roomExists)
> >         return NotFound("Meeting room not found.");
> > 
> >     var reservations = InMemoryStore.Reservations
> >         .Where(r => r.MeetingRoomId == roomId)
> >         .OrderBy(r => r.StartUtc)
> >         .ToList();
> > 
> >     return Ok(reservations);
> > }
> 
> 2. Resulting API Endpoints
> List all reservations
> > GET /api/reservations
> 
> List reservations for a specific room
> > GET /api/reservations/by-room/{roomId}
> 
> 
> Example:
> 
> > GET /api/reservations/by-room/9f3c2c1a-5c1f-4d61-8c1b-22d0f5a18b2a
> 
> 3. Optional (Nice-to-have): Include Room Info in Response
> 
> If you want the response to return room name + capacity alongside reservations, add a projection:
> 
> > var reservations = InMemoryStore.Reservations
> >     .Where(r => r.MeetingRoomId == roomId)
> >     .OrderBy(r => r.StartUtc)
> >     .Select(r => new
> >     {
> >         r.Id,
> >         r.StartUtc,
> >         r.EndUtc,
> >         r.ReservedBy,
> >         Room = InMemoryStore.MeetingRooms.First(m => m.Id == roomId)
> >     })
> >     .ToList();
> 
> 4. Example Response
> > [
> >   {
> >     "id": "d8caa6c1-8fd4-4b61-9ef4-bf3c6d4a0a77",
> >     "startUtc": "2026-02-01T10:00:00Z",
> >     "endUtc": "2026-02-01T11:00:00Z",
> >     "reservedBy": "Alice"
> >   },
> >   {
> >     "id": "a2f73f14-d29f-4b8f-b46f-59f5d87e1a21",
> >     "startUtc": "2026-02-01T13:00:00Z",
> >     "endUtc": "2026-02-01T14:00:00Z",
> >     "reservedBy": "Bob"
> >   }
> > ]
> 
> Why this design works well
> 
> ✔ Keeps rooms and reservations loosely coupled
> ✔ Fast filter logic (single LINQ query)
> ✔ Easy to extend (date ranges, availability, paging)
> 
> If you want next steps, I can:
> 
> Add date range filtering (?from=&to=)
> 
> Add availability lookup per room
> 
> Add room schedule endpoint (/rooms/{id}/schedule)
> 
> Convert to Minimal APIs
> 
> Just tell me what direction you want 🚀

Local database that permanently saves reservations

> Perfect — this is the real step from demo → real app 💪
> We’ll move from EF InMemory to a local persistent database using SQLite, which:
> 
> Runs locally
> 
> Saves reservations permanently
> 
> Requires zero infrastructure
> 
> Works great with .NET 8 + EF Core
> 
> What We’re Changing (High Level)
> Before	After
> EF InMemory	EF Core + SQLite
> Data lost on restart	Data persists on disk
> No migrations	Migrations supported
> 1. Install Required Packages
> > dotnet add package Microsoft.EntityFrameworkCore.Sqlite
> > dotnet add package Microsoft.EntityFrameworkCore.Tools
> 
> 2. Update Program.cs
> 
> Replace the InMemory setup with SQLite.
> 
> Program.cs
> > using MeetingRoomReservationApi.Data;
> > using Microsoft.EntityFrameworkCore;
> > 
> > var builder = WebApplication.CreateBuilder(args);
> > 
> > builder.Services.AddControllers();
> > builder.Services.AddEndpointsApiExplorer();
> > builder.Services.AddSwaggerGen();
> > 
> > builder.Services.AddDbContext<AppDbContext>(options =>
> >     options.UseSqlite("Data Source=meetingrooms.db"));
> > 
> > var app = builder.Build();
> > 
> > // Ensure database + seed data
> > using (var scope = app.Services.CreateScope())
> > {
> >     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
> >     db.Database.Migrate();
> >     DbSeeder.Seed(db);
> > }
> > 
> > app.UseSwagger();
> > app.UseSwaggerUI();
> > 
> > app.UseHttpsRedirection();
> > app.MapControllers();
> > 
> > app.Run();
> 
> 
> 📁 This will create a file called:
> 
> meetingrooms.db
> 
> 
> in your project root.
> 
> 3. Ensure Models Are EF-Friendly
> Models/MeetingRoom.cs
> > namespace MeetingRoomReservationApi.Models;
> > 
> > public class MeetingRoom
> > {
> >     public Guid Id { get; set; } = Guid.NewGuid();
> >     public string Name { get; set; } = string.Empty;
> >     public int Capacity { get; set; }
> > 
> >     public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
> > }
> 
> Models/Reservation.cs
> > namespace MeetingRoomReservationApi.Models;
> > 
> > public class Reservation
> > {
> >     public Guid Id { get; set; } = Guid.NewGuid();
> > 
> >     public Guid MeetingRoomId { get; set; }
> >     public MeetingRoom? MeetingRoom { get; set; }
> > 
> >     public DateTime StartUtc { get; set; }
> >     public DateTime EndUtc { get; set; }
> > 
> >     public string ReservedBy { get; set; } = string.Empty;
> > }
> 
> 4. Update AppDbContext
> Data/AppDbContext.cs
> > using MeetingRoomReservationApi.Models;
> > using Microsoft.EntityFrameworkCore;
> > 
> > namespace MeetingRoomReservationApi.Data;
> > 
> > public class AppDbContext : DbContext
> > {
> >     public AppDbContext(DbContextOptions<AppDbContext> options)
> >         : base(options) { }
> > 
> >     public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();
> >     public DbSet<Reservation> Reservations => Set<Reservation>();
> > 
> >     protected override void OnModelCreating(ModelBuilder modelBuilder)
> >     {
> >         modelBuilder.Entity<MeetingRoom>()
> >             .HasIndex(r => r.Name)
> >             .IsUnique();
> > 
> >         modelBuilder.Entity<Reservation>()
> >             .HasOne(r => r.MeetingRoom)
> >             .WithMany(m => m.Reservations)
> >             .HasForeignKey(r => r.MeetingRoomId)
> >             .OnDelete(DeleteBehavior.Cascade);
> >     }
> > }
> 
> 5. Create the Database (Migrations)
> 
> Run these once:
> 
> > dotnet ef migrations add InitialCreate
> > dotnet ef database update
> 
> 
> You now have:
> 
> Migrations/
> 
> meetingrooms.db (persistent)
> 
> 6. Seeder Still Works (No Changes)
> 
> Your existing DbSeeder will only insert rooms if none exist, so restarts won’t duplicate data.
> 
> ✔ Rooms seeded once
> ✔ Reservations persist forever
> 
> 7. Overlap Validation Still Works (Important)
> 
> Your existing validator is already correct for SQLite:
> 
> > await context.Reservations.AnyAsync(r =>
> >     r.MeetingRoomId == reservation.MeetingRoomId &&
> >     r.Id != reservation.Id &&
> >     reservation.StartUtc < r.EndUtc &&
> >     reservation.EndUtc > r.StartUtc
> > );
> 
> 
> SQLite handles this cleanly.
> 
> 8. What You Have Now
> 
> ✅ Local database file
> ✅ Persistent reservations
> ✅ Relational integrity
> ✅ EF migrations
> ✅ Easy upgrade path to SQL Server / Postgres
> 
> Optional (Very Recommended Next Steps)
> 
> If you want to go further, I can:
> 
> 🔒 Add database-level constraints for overlaps
> 
> ⏱ Add availability search
> 
> 🧪 Add integration tests using SQLite in-memory
> 
> 🐳 Add Docker + volume persistence
> 
> 🌍 Add timezone-safe reservations
> 
> Just say where you want to take it next 🚀