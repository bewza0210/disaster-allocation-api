using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisasterApi.Models;

public class Assignment
{
    [Key]
    public int id { get; set; }

    [ForeignKey("Truck")]
    public int TruckID { get; set; }
    public Truck Truck { get; set; } = null!;

    [ForeignKey("Area")]
    public int AreaID { get; set; }
    public Area Area { get; set; } = null!;
    
    public Dictionary<string, int> RequireDelivered { get; set; } = new();
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
