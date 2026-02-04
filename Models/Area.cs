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

    public required string AreaID { get; set; }

    public int UrgentyLevel { get; set; }

    public Dictionary<string, int> RequireResources { get; set; } = new();

    [Range(1, int.MaxValue, ErrorMessage = "TimeConstraint must be greater than 0")]
    public int TimeConstraint { get; set; }

    [JsonIgnore]
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}
