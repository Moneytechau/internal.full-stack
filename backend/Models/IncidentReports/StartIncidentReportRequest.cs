using System.ComponentModel.DataAnnotations;

namespace Backend.Models.IncidentReports;

public class StartIncidentReportRequest
{
    [Required, MinLength(2)]
    public string FullName { get; set; } = string.Empty;

    [Required, RegularExpression(@"^[0-9\s]{8,15}$")]
    public string Mobile { get; set; } = string.Empty;
}
