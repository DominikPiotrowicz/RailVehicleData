namespace RailVehicleData.Application.Dto
{
	/// <summary>
	/// DTO dla wyświetlania danych pojazdu szynowego
	/// </summary>
	public class VehicleDto
	{
		public int VehicleId { get; set; }
		public string? Model { get; set; }
		public string? Manufacturer { get; set; }
		public int? ProductionYear { get; set; }
		public int? MaxSpeed { get; set; }
		public decimal? Weight { get; set; }
		public decimal? Length { get; set; }
		public decimal? Width { get; set; }
		public decimal? Height { get; set; }
		public string? PowerType { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }

		// Właściwości dla pojazdów elektrycznych (opcjonalne)
		public string? Voltage { get; set; }
		public string? CurrentType { get; set; }
		public decimal? EnginePower { get; set; }
		public int? NumberOfMotors { get; set; }
		public string? MotorType { get; set; }
		public string? WheelArrangement { get; set; }
		public bool? IsMultiSystem { get; set; }
		public decimal? MaxTractiveEffort { get; set; }
	}
}
