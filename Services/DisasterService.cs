using DisasterApi.Data;
using DisasterApi.Models;

namespace DisasterApi.Services;

public class DisasterService
{
    private readonly AppDbContext _context;

    public DisasterService(AppDbContext context)
    {
        _context = context;
    }

    public (bool success, string message) AddAffectedArea(Area area)
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
                return (true, $"AreaID '{area.AreaID}' updated successfully");
            }

            _context.Areas.Add(area);
            _context.SaveChanges();
            return (true, $"AreaID '{area.AreaID}' created successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Error processing Area: {ex.Message}");
        }
    }

    public (bool success, string message) AddResourceTruck(Truck truck)
    {
        try
        {
            var existing = _context.Trucks.FirstOrDefault(t => t.TruckID == truck.TruckID);

            if (existing != null)
            {
                existing.AvailableResources = truck.AvailableResources;
                existing.TravelTimeToArea = truck.TravelTimeToArea;
                _context.SaveChanges();
                return (true, $"TruckID '{truck.TruckID}' updated successfully");
            }

            _context.Trucks.Add(truck);
            _context.SaveChanges();
            return (true, $"TruckID '{truck.TruckID}' created successfully");
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