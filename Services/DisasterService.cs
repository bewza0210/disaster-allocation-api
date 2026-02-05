using DisasterApi.Data;
using DisasterApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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
        _AssignmentCacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) };
    }

    public async Task<(int statusCode, bool success, string message)> AddAffectedArea(Area area)
    {
        try
        {
            var existing = _context.Areas.FirstOrDefault(a => a.AreaID == area.AreaID);

            if (existing != null)
            {
                existing.UrgentyLevel = area.UrgentyLevel;
                existing.RequireResources = area.RequireResources;
                existing.TimeConstraint = area.TimeConstraint;
                _context.SaveChanges();
                return (200, true, $"AreaID '{area.AreaID}' updated successfully");
            }

            _context.Areas.Add(area);
            _context.SaveChanges();
            return (201, true, $"AreaID '{area.AreaID}' created successfully");
        }
        catch (Exception ex)
        {
            return (500, false, $"Error processing Area: {ex.Message}");
        }
    }

    public async Task<(int statusCode, bool success, string message)> AddResourceTruck(Truck truck)
    {
        try
        {
            var existing = _context.Trucks.FirstOrDefault(t => t.TruckID == truck.TruckID);

            if (existing != null)
            {
                existing.AvailableResources = truck.AvailableResources;
                existing.TravelTimeToArea = truck.TravelTimeToArea;
                _context.SaveChanges();
                return (200, true, $"TruckID '{truck.TruckID}' updated successfully");
            }

            _context.Trucks.Add(truck);
            _context.SaveChanges();
            return (201, true, $"TruckID '{truck.TruckID}' created successfully");
        }
        catch (Exception ex)
        {
            return (500, false, $"Error processing Truck: {ex.Message}");
        }
    }

    public async Task<(int statusCode, bool success, string message)> GenerateAssignments()
    {
        try
        {
            var areas = _context.Areas.OrderByDescending(a => a.UrgentyLevel).ToList();
            if (!areas.Any())
                return (404, false, "No areas found");

            var trucks = _context.Trucks.ToList();
            if (!trucks.Any())
                return (404, false, "No trucks found");

            var assignmentsCreated = 0;

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

                    assignmentsCreated++;
                }
            }

            if (assignmentsCreated == 0)
                return (400, false, "No matching trucks found for any area");

            _context.SaveChanges();

            var assignments = _context.Assignments.Select(a => new
            {
                AreaID = a.Area.AreaID,
                TruckID = a.Truck.TruckID,
                DeliveredResource = a.RequireDelivered
            }).ToList();

            await _cache.SetStringAsync(
                "assignments:all",
                JsonSerializer.Serialize(assignments),
                _AssignmentCacheOptions);

            return (201, true, $"Generated {assignmentsCreated} assignments successfully");
        }
        catch (Exception ex)
        {
            return (500, false, $"Error generating assignments: {ex.Message}");
        }
    }

    public async Task<(int statusCode, bool success, string message, object? data)> GetAssignments()
    {
        try
        {
            var cached = await _cache.GetStringAsync("assignments:all");
            if (cached != null)
            {
                var cachedData = JsonSerializer.Deserialize<object>(cached);
                return (200, true, "Retrieved from cache", cachedData);
            }

            var assignments = _context.Assignments.Select(a => new
            {
                AreaID = a.Area.AreaID,
                TruckID = a.Truck.TruckID,
                DeliveredResource = a.RequireDelivered
            }).ToList();

            if (!assignments.Any())
                return (404, true, "No assignments found", new List<object>());

            await _cache.SetStringAsync(
                "assignments:all",
                JsonSerializer.Serialize(assignments),
                _AssignmentCacheOptions);

            return (200, true, "Retrieved from database", assignments);
        }
        catch (Exception ex)
        {
            return (500, false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(int statusCode, bool success, string message)> DeleteAssignments()
    {
        try
        {
            var assignments = _context.Assignments.ToList();
            if (!assignments.Any())
                return (404, true, "No assignments to delete");

            _context.Assignments.RemoveRange(assignments);
            _context.SaveChanges();

            await _cache.RemoveAsync("assignments:all");

            return (200, true, $"Deleted {assignments.Count} assignments successfully");
        }
        catch (Exception ex)
        {
            return (500, false, $"Error deleting assignments: {ex.Message}");
        }
    }

    private Truck? FindMatchingTruck(Area area, List<Truck> trucks)
    {
        foreach (var truck in trucks)
        {
            var allKeysMatch = area.RequireResources.Keys
                .All(key => truck.AvailableResources.ContainsKey(key));
            if (!allKeysMatch)
                continue;

            var canFulfill = area.RequireResources
                .All(req => truck.AvailableResources[req.Key] >= req.Value);
            if (!canFulfill)
                continue;

            if (!truck.TravelTimeToArea.TryGetValue(area.AreaID, out var travelTime))
                continue;

            if (travelTime > area.TimeConstraint)
                continue;

            return truck;
        }

        return null;
    }
}