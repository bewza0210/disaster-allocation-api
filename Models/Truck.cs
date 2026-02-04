using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DisasterApi.Models;

/// <summary>
/// รถขนส่งทรัพยากร
/// </summary>
public class Truck
{
    [Key]
    [JsonIgnore]
    public int id { get; set; }

    public string AreaID { get; set; }

    public Dictionary<string, int> AvailableResources { get; set; } = new();

    public Dictionary<string, int> TravelTimeToArea { get; set; } = new();

    [JsonIgnore]
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}
