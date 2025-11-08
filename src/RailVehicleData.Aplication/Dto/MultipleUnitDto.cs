namespace RailVehicleData.Aplication.Dto;

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
    public string Designation { get; set; }
    public int CarCount { get; set; }
    public int FirstCarNumber { get; set; }
    public int LastCarNumber { get; set; }
    public int TotalCapacity { get; set; }
    public DateTime CommissionedDate { get; set; }
    public decimal TotalLengthMeters { get; set; }
    public decimal MaxSpeedKmH { get; set; }
    public string Type { get; set; }
}
