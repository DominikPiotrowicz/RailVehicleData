namespace RailVehicleData.Domain.Entities
{
	public class DieselVehicle : Vehicle
	{
        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
