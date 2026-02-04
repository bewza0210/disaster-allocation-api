using Microsoft.EntityFrameworkCore;
using DisasterApi.Models;
using System.Text.Json;

namespace DisasterApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Truck> Trucks { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Assignment> Assignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // JSON Conversions
        modelBuilder.Entity<Area>()
            .Property(a => a.RequireResources)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions)null!) ?? new()
            );

        modelBuilder.Entity<Truck>()
            .Property(t => t.AvailableResources)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions)null!) ?? new()
            );

        modelBuilder.Entity<Truck>()
            .Property(t => t.TravelTimeToArea)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions)null!) ?? new()
            );

        modelBuilder.Entity<Assignment>()
            .Property(a => a.RequireDelivered)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions)null!) ?? new()
            );

        // Default Timestamps
        modelBuilder.Entity<Area>()
            .Property(a => a.CreateAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Truck>()
            .Property(t => t.CreateAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Assignment>()
            .Property(a => a.AssignedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        // Relationships
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Truck)
            .WithMany()
            .HasForeignKey(a => a.TruckID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Area)
            .WithMany()
            .HasForeignKey(a => a.AreaID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}