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

    public void AddAffectedArea(Area area)
    {
        _context.Areas.Add(area);
        _context.SaveChanges();
    }

    public void AddResourceTruck(Truck truck)
    {
        _context.Trucks.Add(truck);
        _context.SaveChanges();
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