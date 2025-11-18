using System.ComponentModel.DataAnnotations;

namespace RailVehicleData.Application.Dto;

/// <summary>
/// Data Transfer Object for MultipleUnit (EMU/DMU).
/// </summary>
public class MultipleUnitDto
{
    public Guid MultipleUnitId { get; set; }
    public string Designation { get; set; }
    public int CarCount { get; set; }
    public int FirstCarNumber { get; set; }
    public int LastCarNumber { get; set; }
    public int TotalCapacity { get; set; }
    public DateTime CommissionedDate { get; set; }
    public DateTime? DecommissionedDate { get; set; }
    public bool IsActive { get; set; }
    public decimal TotalLengthMeters { get; set; }
    public decimal MaxSpeedKmH { get; set; }
    public string Type { get; set; }
    public int CarIdsCount { get; set; }
    public bool IsComplete { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new MultipleUnit.
/// </summary>
public class NewMultipleUnitDto
{
    [Required(ErrorMessage = "Designation is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Designation must be between 1 and 50 characters")]
    public string Designation { get; set; }

    [Range(1, 50, ErrorMessage = "CarCount must be between 1 and 50")]
    public int CarCount { get; set; }

    [Range(0, 100000, ErrorMessage = "FirstCarNumber must be between 0 and 100000")]
    public int FirstCarNumber { get; set; }

    [Range(0, 100000, ErrorMessage = "LastCarNumber must be between 0 and 100000")]
    public int LastCarNumber { get; set; }

    [Range(1, 10000, ErrorMessage = "TotalCapacity must be between 1 and 10000")]
    public int TotalCapacity { get; set; }

    public DateTime CommissionedDate { get; set; }

    [Range(10, 1000, ErrorMessage = "TotalLengthMeters must be between 10 and 1000")]
    public decimal TotalLengthMeters { get; set; }

    [Range(1, 500, ErrorMessage = "MaxSpeedKmH must be between 1 and 500")]
    public decimal MaxSpeedKmH { get; set; }

    [Required(ErrorMessage = "Type is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Type must be between 1 and 20 characters")]
    public string Type { get; set; }
}
