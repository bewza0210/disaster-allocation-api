using DisasterApi.Data;
using DisasterApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DisasterApi.Services;

public class DisasterService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _AssignmentCacheOptions;

    public DisasterService(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
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

            return (true, $"TruckID '{truck.TruckID}' successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Error processing Truck: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> GenerateAssignments()
    {
        try
        {
            var areas = _context.Areas.OrderByDescending(a => a.UrgentyLevel).ToList();
            if (!areas.Any()) return (false, "No areas found");

            var trucks = _context.Trucks.ToList();
            if (!trucks.Any()) return (false, "No trucks found");

            foreach (var area in areas)
            {
                var matchedTruck = FindMatchingTruck(area, trucks);
                if (matchedTruck != null)
                {
                    var assignment = new Assignment
                    {
                        TruckID = matchedTruck.id,
                        AreaID = area.id,
                        RequireDelivered = new Dictionary<string, int>(area.RequireResources)
                    };
                    _context.Assignments.Add(assignment);
                    foreach (var resource in area.RequireResources) 
                        matchedTruck.AvailableResources[resource.Key] -= resource.Value;
                }
            }

            _context.SaveChanges();

            // Cache assignments
            var assignments = _context.Assignments.ToList();
            await _cache.SetStringAsync(
                "assignments:all",
                JsonSerializer.Serialize(assignments),
                _AssignmentCacheOptions);

            return (true, $"Generate assignments success");
        }
        catch (Exception ex)
        {
            return (false, $"Error Generate assignment: {ex.Message}");
        }
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


    /// <summary>
    /// หา Truck ที่ match กับ Area
    /// - ต้องมี resource keys ครบทุกตัวที่ Area ต้องการ
    /// - AvailableResources >= RequireResources ทุก key
    /// </summary>
    private Truck? FindMatchingTruck(Area area, List<Truck> trucks)
    {
        foreach (var truck in trucks)
        {
            // 1. เช็คว่า truck มี keys ครบทุกตัวที่ area ต้องการไหม
            var allKeysMatch = area.RequireResources.Keys
                .All(key => truck.AvailableResources.ContainsKey(key));
            if (!allKeysMatch)
                continue;

            // 2. เช็คว่า AvailableResources >= RequireResources ทุก key ไหม
            var canFulfill = area.RequireResources
                .All(req => truck.AvailableResources[req.Key] >= req.Value);
            if (!canFulfill)
                continue;

            // 3. เช็คว่า TravelTime <= TimeConstraint ไหม
            if (!truck.TravelTimeToArea.TryGetValue(area.AreaID, out var travelTime))
                continue;  // ถ้าไม่มี TravelTime ไปยัง area นี้ ข้าม

            if (travelTime > area.TimeConstraint)
                continue;  // ถ้าเดินทางนานเกินไป ข้าม

            return truck;
        }

        return null;
    }
}