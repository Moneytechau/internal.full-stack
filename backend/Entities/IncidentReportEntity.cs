namespace Backend.Entities;

public class IncidentReportEntity
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string? IncidentType { get; set; }

    public decimal? EstimatedDamage { get; set; }

    public DateOnly? IncidentDate { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
