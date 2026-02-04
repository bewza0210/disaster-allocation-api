using System.ComponentModel.DataAnnotations;

namespace DisasterApi.Models;

public class Area
{
    [Key]
    public int id { get; set; }
    public int UrgentyLevel { get; set; } = 0;
    public Dictionary<string, int> RequireResources { get; set; } = new();
    public int TimeConstraint { get; set; } = 0;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}
