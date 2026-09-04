using System.ComponentModel.DataAnnotations;

namespace Backend.Models.IncidentReports;

public class UpdateIncidentRequest
{
    [Required]
    public string IncidentType { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal EstimatedDamage { get; set; }
}
