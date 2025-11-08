namespace RailVehicleData.Domain.Entities
{
	public class StreamVehicle 
	{
        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
