namespace RailVehicleData.Domain.Entities
{
	public class VehicleType
	{
		public int VehicleTypeId{ get; set; }
		public string VehicleTypeName { get; set; }

		public virtual ICollection<Vehicle> Vehicles { get; set; }
	}
}
