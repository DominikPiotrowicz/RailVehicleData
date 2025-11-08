namespace RailVehicleData.Domain.Entities
{
	public class ElectricMultipleUnit : Vehicle
	{
        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
