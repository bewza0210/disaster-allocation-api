using System.ComponentModel.DataAnnotations;

namespace DisasterApi.Models;

public class Truck
{
    [Key]
    public int id { get; set; }
    public Dictionary<string, int> AvailableResources { get; set; } = new();
    public Dictionary<string, int> TravelTimeToArea { get; set; } = new();
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}
