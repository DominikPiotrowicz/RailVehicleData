namespace RailVehicleData.Aplication.Dto
{
	public class VehicleDto
	{
		public int VehicleId { get; set; }
		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public int Year { get; set; }
		public string Name { get; set; }
		public int ServiceWeight { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int WheelDiameter { get; set; }
		// Dodaj inne właściwości pojazdu, jeśli są potrzebne
	}
}
