using MeetingRoomApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeetingRoom>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.MeetingRoom)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.MeetingRoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}