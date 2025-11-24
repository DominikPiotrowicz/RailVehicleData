using System.ComponentModel.DataAnnotations;

namespace RailVehicleData.Application.Dto;

/// <summary>
/// Data Transfer Object for creating a new Vehicle.
/// </summary>
public class NewVehicleDto
{
    [Required(ErrorMessage = "Manufacturer is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Manufacturer must be between 1 and 100 characters")]
    public string Manufacturer { get; set; }

    [Required(ErrorMessage = "Model is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Model must be between 1 and 100 characters")]
    public string Model { get; set; }

    [Range(1800, 2100, ErrorMessage = "ManufacturedYear must be between 1800 and 2100")]
    public int ManufacturedYear { get; set; }

    public Guid? SeriesId { get; set; }
    public Guid? MultipleUnitId { get; set; }

    public DateTime CommissionedDate { get; set; }

    // CommonSpecifications (in meters, tons, km/h for API)
    [Range(0.5, 200, ErrorMessage = "LengthMeters must be between 0.5 and 200")]
    public decimal LengthMeters { get; set; }

    [Range(1, 4, ErrorMessage = "WidthMeters must be between 1 and 4")]
    public decimal WidthMeters { get; set; }

    [Range(2, 6, ErrorMessage = "HeightMeters must be between 2 and 6")]
    public decimal HeightMeters { get; set; }

    [Range(0.3, 2, ErrorMessage = "WheelDiameterMeters must be between 0.3 and 2")]
    public decimal WheelDiameterMeters { get; set; }

    [Range(1, 30, ErrorMessage = "AxleCount must be between 1 and 30")]
    public int AxleCount { get; set; }

    [Range(0.1, 500, ErrorMessage = "ServiceWeightTons must be between 0.1 and 500")]
    public decimal ServiceWeightTons { get; set; }

    [Range(1, 500, ErrorMessage = "MaxSpeedKmH must be between 1 and 500")]
    public decimal MaxSpeedKmH { get; set; }

    [Range(0, 1000, ErrorMessage = "NumberOfSeats must be between 0 and 1000")]
    public int? NumberOfSeats { get; set; }

    [Range(0, 100, ErrorMessage = "NumberOfDoors must be between 0 and 100")]
    public int? NumberOfDoors { get; set; }

    [Range(0, 50000, ErrorMessage = "ToiletCapacityLiters must be between 0 and 50000")]
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
    [Range(1, 20000, ErrorMessage = "MaxPowerKw must be between 1 and 20000")]
    public int MaxPowerKw { get; set; }

    [Required(ErrorMessage = "VoltageSystem is required")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "VoltageSystem must be between 1 and 30 characters")]
    public string VoltageSystem { get; set; }

    [Range(40, 150, ErrorMessage = "ACFrequencyHz must be between 40 and 150")]
    public int? ACFrequencyHz { get; set; }

    public bool HasRegenerativeBraking { get; set; }

    [Required(ErrorMessage = "PantographType is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "PantographType must be between 1 and 50 characters")]
    public string PantographType { get; set; }
}

/// <summary>
/// DTO for creating new Diesel Traction System.
/// </summary>
public class NewDieselTractionDto : NewTractionSystemDto
{
    [Range(1, 10000, ErrorMessage = "MaxPowerKw must be between 1 and 10000")]
    public int MaxPowerKw { get; set; }

    [Range(1, 100000, ErrorMessage = "FuelCapacityLiters must be between 1 and 100000")]
    public decimal FuelCapacityLiters { get; set; }

    [Required(ErrorMessage = "EngineType is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "EngineType must be between 1 and 50 characters")]
    public string EngineType { get; set; }

    [Required(ErrorMessage = "EmissionStandard is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "EmissionStandard must be between 1 and 20 characters")]
    public string EmissionStandard { get; set; }

    [Range(1, 30, ErrorMessage = "CylinderCount must be between 1 and 30")]
    public int CylinderCount { get; set; }

    [Range(0.1, 150, ErrorMessage = "FuelConsumptionPer100Km must be between 0.1 and 150")]
    public decimal? FuelConsumptionPer100Km { get; set; }
}

/// <summary>
/// DTO for creating new Steam Traction System.
/// </summary>
public class NewSteamTractionDto : NewTractionSystemDto
{
    [Range(0.1, 30, ErrorMessage = "BoilerWorkingPressureBars must be between 0.1 and 30")]
    public decimal BoilerWorkingPressureBars { get; set; }

    [Range(1, 50, ErrorMessage = "BoilerMaxPressureBars must be between 1 and 50")]
    public decimal BoilerMaxPressureBars { get; set; }

    [Range(1, 100000, ErrorMessage = "BoilerCapacityLiters must be between 1 and 100000")]
    public decimal BoilerCapacityLiters { get; set; }

    [Range(0.1, 100, ErrorMessage = "GrateAreaSquareMeters must be between 0.1 and 100")]
    public decimal GrateAreaSquareMeters { get; set; }

    [Required(ErrorMessage = "FireboxType is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "FireboxType must be between 1 and 50 characters")]
    public string FireboxType { get; set; }

    [Required(ErrorMessage = "FuelType is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "FuelType must be between 1 and 50 characters")]
    public string FuelType { get; set; }

    [Range(1, 10000, ErrorMessage = "HeatingAreaSquareMeters must be between 1 and 10000")]
    public decimal? HeatingAreaSquareMeters { get; set; }
}
