using DisasterApi.Data;
using DisasterApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DisasterApi.Services;

public class DisasterService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _AddCacheOptions;
    private readonly DistributedCacheEntryOptions _AssignmentCacheOptions;

    public DisasterService(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
        _AddCacheOptions = new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)};
        _AssignmentCacheOptions = new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) };
    }

    public async Task<(bool success, string message)> AddAffectedArea(Area area)
    {
        try
        {
            var existing = _context.Areas.FirstOrDefault(a => a.AreaID == area.AreaID);

            if (existing != null)
            {
                existing.UrgentyLevel = area.UrgentyLevel;
                existing.RequireResources = area.RequireResources;
                existing.TimeConstraint = area.TimeConstraint;
            }
            else
            {
                _context.Areas.Add(area);
                existing = area;
            }

            _context.SaveChanges();

            await _cache.SetStringAsync(
                $"area:{area.AreaID}",
                JsonSerializer.Serialize(existing),
                _AddCacheOptions);

            return (true, $"AreaID '{area.AreaID}' successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Error processing Area: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> AddResourceTruck(Truck truck)
    {
        try
        {
            var existing = _context.Trucks.FirstOrDefault(t => t.TruckID == truck.TruckID);

            if (existing != null)
            {
                existing.AvailableResources = truck.AvailableResources;
                existing.TravelTimeToArea = truck.TravelTimeToArea;
            }
            else
            {
                _context.Trucks.Add(truck);
                existing = truck;
            }

            _context.SaveChanges();

            await _cache.SetStringAsync(
                $"truck:{truck.TruckID}",
                JsonSerializer.Serialize(existing),
                _AddCacheOptions);

            return (true, $"TruckID '{truck.TruckID}' successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Error processing Truck: {ex.Message}");
        }
    }

    public void GenerateAssignments()
    {
        // Placeholder for assignment generation logic
        // This would typically involve complex logic to match resources to affected areas
    }

    public List<Assignment> GetAssignments()
    {
        return _context.Assignments.ToList();
    }

    public void DeleteAssignments()
    {
        var assignments = _context.Assignments.ToList();
        _context.Assignments.RemoveRange(assignments);
        _context.SaveChanges();
    }
}