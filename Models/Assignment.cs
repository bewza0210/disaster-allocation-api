using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DisasterApi.Models;

/// <summary>
/// งานที่มอบหมายให้รถขนส่งไปยังพื้นที่ประสบภัย
/// </summary>
public class Assignment
{
    [Key]
    [JsonIgnore]
    public int id { get; set; }

    [ForeignKey("Truck")]
    public int TruckID { get; set; }
    public Truck Truck { get; set; } = null!;

    [ForeignKey("Area")]
    public int AreaID { get; set; }
    public Area Area { get; set; } = null!;
    
    public Dictionary<string, int> RequireDelivered { get; set; } = new();

    [JsonIgnore]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
