namespace RailVehicleData.Application.Dto;

/// <summary>
/// Base DTO for traction systems.
/// </summary>
public abstract class TractionSystemDto
{
    public Guid TractionSystemId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime InstalledDate { get; set; }
    public DateTime? RemovedDate { get; set; }
    public int? MaxPowerKw { get; set; }
    public bool IsActive { get; set; }
    public string Type { get; set; }
}

/// <summary>
/// DTO for Electric Traction System.
/// </summary>
public class ElectricTractionDto : TractionSystemDto
{
    public string VoltageSystem { get; set; }
    public int? ACFrequencyHz { get; set; }
    public bool HasRegenerativeBraking { get; set; }
    public string PantographType { get; set; }
}

/// <summary>
/// DTO for Diesel Traction System.
/// </summary>
public class DieselTractionDto : TractionSystemDto
{
    public decimal FuelCapacityLiters { get; set; }
    public string EngineType { get; set; }
    public string EmissionStandard { get; set; }
    public int CylinderCount { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
}

/// <summary>
/// DTO for Steam Traction System.
/// </summary>
public class SteamTractionDto : TractionSystemDto
{
    public decimal BoilerWorkingPressureBars { get; set; }
    public decimal BoilerMaxPressureBars { get; set; }
    public decimal BoilerCapacityLiters { get; set; }
    public decimal GrateAreaSquareMeters { get; set; }
    public string FireboxType { get; set; }
    public string FuelType { get; set; }
    public decimal? HeatingAreaSquareMeters { get; set; }
}
