namespace RailVehicleData.Aplication.Dto
{
	public class NewVehicleDto
	{
		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public int Year { get; set; }

		public string VehicleTypeName { get; set; }
		public string SeriesName { get; set; }
		public int ServiceWeight { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int WheelDiameter { get; set; }
	}
}
