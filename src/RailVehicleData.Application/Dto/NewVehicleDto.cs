namespace RailVehicleData.Application.Dto;

/// <summary>
/// Data Transfer Object for creating a new Vehicle.
/// </summary>
public class NewVehicleDto
{
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public int ManufacturedYear { get; set; }
    public Guid? SeriesId { get; set; }
    public Guid? MultipleUnitId { get; set; }
    public DateTime CommissionedDate { get; set; }

    // CommonSpecifications (in meters, kg, km/h for API)
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

    // Traction systems to add
    public IEnumerable<NewTractionSystemDto> TractionSystems { get; set; } = new List<NewTractionSystemDto>();
}

/// <summary>
/// Base DTO for creating new traction systems.
/// </summary>
public abstract class NewTractionSystemDto
{
    public DateTime InstalledDate { get; set; }
}

/// <summary>
/// DTO for creating new Electric Traction System.
/// </summary>
public class NewElectricTractionDto : NewTractionSystemDto
{
    public int MaxPowerKw { get; set; }
    public string VoltageSystem { get; set; }
    public int? ACFrequencyHz { get; set; }
    public bool HasRegenerativeBraking { get; set; }
    public string PantographType { get; set; }
}

/// <summary>
/// DTO for creating new Diesel Traction System.
/// </summary>
public class NewDieselTractionDto : NewTractionSystemDto
{
    public int MaxPowerKw { get; set; }
    public decimal FuelCapacityLiters { get; set; }
    public string EngineType { get; set; }
    public string EmissionStandard { get; set; }
    public int CylinderCount { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
}

/// <summary>
/// DTO for creating new Steam Traction System.
/// </summary>
public class NewSteamTractionDto : NewTractionSystemDto
{
    public decimal BoilerWorkingPressureBars { get; set; }
    public decimal BoilerMaxPressureBars { get; set; }
    public decimal BoilerCapacityLiters { get; set; }
    public decimal GrateAreaSquareMeters { get; set; }
    public string FireboxType { get; set; }
    public string FuelType { get; set; }
    public decimal? HeatingAreaSquareMeters { get; set; }
}
