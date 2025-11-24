namespace RailVehicleData.Application.Dto;

/// <summary>
/// Data Transfer Object for Vehicle.
/// Used for API responses.
/// </summary>
public class VehicleDto
{
    public Guid VehicleId { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public int ManufacturedYear { get; set; }
    public Guid? SeriesId { get; set; }
    public string Role { get; set; }
    public Guid? MultipleUnitId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CommissionedDate { get; set; }
    public DateTime? DecommissionedDate { get; set; }

    // CommonSpecifications
    public decimal LengthMeters { get; set; }
    public decimal WidthMeters { get; set; }
    public decimal HeightMeters { get; set; }
    public decimal WheelDiameterMeters { get; set; }
    public int AxleCount { get; set; }
    public decimal ServiceWeightTons { get; set; }
    public decimal MaxSpeedKmH { get; set; }
    public int? NumberOfSeats { get; set; }
    public int? NumberOfDoors { get; set; }
    public decimal? ToiletCapacityLiters { get; set; }
    public bool HasClimateControl { get; set; }

    // Traction Systems
    public IEnumerable<TractionSystemDto> TractionSystems { get; set; } = new List<TractionSystemDto>();
    public decimal? TotalActivePowerKw { get; set; }
}
