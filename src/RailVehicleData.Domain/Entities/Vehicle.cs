namespace RailVehicleData.Domain.Entities
{
	public class Vehicle
	{
		public int VehicleId { get; set; }
		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public int Year { get; set; }


		public int VehicleTypeId { get; set; }
		public virtual VehicleType	VehicleType { get; set; }

		public virtual ElectricVehicle ElectricVehicle { get; set; }
	}
}
