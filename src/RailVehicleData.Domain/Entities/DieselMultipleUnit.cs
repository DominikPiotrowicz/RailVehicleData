namespace RailVehicleData.Domain.Entities
{
	public class DieselMultipleUnit : Vehicle
	{
        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
