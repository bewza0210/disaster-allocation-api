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
}