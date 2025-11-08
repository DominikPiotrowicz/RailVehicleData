namespace RailVehicleData.Domain.Entities
{
	public class ElectricVehicle : Vehicle
	{
		public int VehicleId { get; set; }

		public virtual Vehicle Vehicle { get; set; }
	}
}
