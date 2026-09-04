using Backend.Entities;
using Backend.Models.IncidentReports;
using Backend.Repositories;
using Backend.Services;
using Moq;

namespace Backend.UnitTests.Services;

public class IncidentReportServiceTests
{
    private readonly Mock<IIncidentReportRepository> _repository = new();
    private readonly IncidentReportService _sut;

    public IncidentReportServiceTests()
    {
        _sut = new IncidentReportService(_repository.Object);
    }

    [Fact]
    public async Task StartAsync_AddsEntityAndReturnsResponse()
    {
        var request = new StartIncidentReportRequest
        {
            FullName = "Jane Doe",
            Mobile = "0412345678",
        };

        IncidentReportEntity? added = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<IncidentReportEntity>(), It.IsAny<CancellationToken>()))
            .Callback<IncidentReportEntity, CancellationToken>((entity, _) => added = entity)
            .ReturnsAsync((IncidentReportEntity entity, CancellationToken _) => entity);

        var result = await _sut.StartAsync(request, CancellationToken.None);

        Assert.NotNull(added);
        Assert.NotEqual(Guid.Empty, added!.Id);
        Assert.Equal(request.FullName, added.FullName);
        Assert.Equal(request.Mobile, added.Mobile);
        Assert.Equal(added.CreatedAtUtc, added.UpdatedAtUtc);

        Assert.Equal(added.Id, result.Id);
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.Mobile, result.Mobile);
        Assert.Null(result.IncidentType);
        Assert.Null(result.EstimatedDamage);

        _repository.Verify(r => r.AddAsync(It.IsAny<IncidentReportEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateReporterDetailsAsync_WhenEntityExists_UpdatesAndReturnsResponse()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var entity = new IncidentReportEntity
        {
            Id = id,
            FullName = "Jane Doe",
            Mobile = "0412345678",
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = createdAt,
        };

        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var request = new StartIncidentReportRequest
        {
            FullName = "Jane Smith",
            Mobile = "0498765432",
        };

        var result = await _sut.UpdateReporterDetailsAsync(id, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.Mobile, result.Mobile);
        Assert.Equal(request.FullName, entity.FullName);
        Assert.Equal(request.Mobile, entity.Mobile);
        Assert.True(entity.UpdatedAtUtc > createdAt);

        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.AddAsync(It.IsAny<IncidentReportEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateReporterDetailsAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncidentReportEntity?)null);

        var result = await _sut.UpdateReporterDetailsAsync(id, new StartIncidentReportRequest(), CancellationToken.None);

        Assert.Null(result);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateIncidentAsync_WhenEntityExists_UpdatesAndReturnsResponse()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var entity = new IncidentReportEntity
        {
            Id = id,
            FullName = "Jane Doe",
            Mobile = "0412345678",
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = createdAt,
        };

        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var request = new UpdateIncidentRequest
        {
            IncidentType = "Theft",
            EstimatedDamage = 1500m,
        };

        var result = await _sut.UpdateIncidentAsync(id, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(request.IncidentType, result!.IncidentType);
        Assert.Equal(request.EstimatedDamage, result.EstimatedDamage);
        Assert.Equal(request.IncidentType, entity.IncidentType);
        Assert.Equal(request.EstimatedDamage, entity.EstimatedDamage);
        Assert.True(entity.UpdatedAtUtc > createdAt);

        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateIncidentAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncidentReportEntity?)null);

        var result = await _sut.UpdateIncidentAsync(id, new UpdateIncidentRequest(), CancellationToken.None);

        Assert.Null(result);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateDetailsAsync_WhenEntityExists_UpdatesAndReturnsResponse()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var entity = new IncidentReportEntity
        {
            Id = id,
            FullName = "Jane Doe",
            Mobile = "0412345678",
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = createdAt,
        };

        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var request = new UpdateIncidentDetailsRequest
        {
            IncidentDate = new DateOnly(2026, 1, 15),
            Location = "Sydney",
            Description = "Vehicle collision at intersection.",
        };

        var result = await _sut.UpdateDetailsAsync(id, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(request.IncidentDate, result!.IncidentDate);
        Assert.Equal(request.Location, result.Location);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.IncidentDate, entity.IncidentDate);
        Assert.Equal(request.Location, entity.Location);
        Assert.Equal(request.Description, entity.Description);
        Assert.True(entity.UpdatedAtUtc > createdAt);

        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateDetailsAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncidentReportEntity?)null);

        var result = await _sut.UpdateDetailsAsync(id, new UpdateIncidentDetailsRequest(), CancellationToken.None);

        Assert.Null(result);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
