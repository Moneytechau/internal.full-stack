using System.ComponentModel.DataAnnotations;

namespace Backend.Models.IncidentReports;

public class UpdateIncidentDetailsRequest
{
    [Required]
    public DateOnly IncidentDate { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;
}
