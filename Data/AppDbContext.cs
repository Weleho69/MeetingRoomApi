using MeetingRoomApi.Models;
using MeetingRoomAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<Customer> Customers => Set<Customer>();

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


        // Reservation → Customer FK via Email
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerEmail)
            .OnDelete(DeleteBehavior.Restrict);

        // Customer email is PK
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.Email);
    }
}