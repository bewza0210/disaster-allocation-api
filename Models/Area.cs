using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DisasterApi.Models;

/// <summary>
/// พื้นที่ประสบภัย
/// </summary>
public class Area
{
    [Key]
    [JsonIgnore]
    public int id { get; set; }

    public string AreaID { get; set; }

    public int UrgentyLevel { get; set; } = 0;

    public Dictionary<string, int> RequireResources { get; set; } = new();

    public int TimeConstraint { get; set; } = 0;

    [JsonIgnore]
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}
