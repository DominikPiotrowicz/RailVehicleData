using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Domain.Interfaces
{
	public interface IVehicleRepository
	{
		Task<List<Vehicle>> GetAllVehiclesAsync();
		Task<Vehicle> GetByIdAsync(int id);
		void Add (Vehicle vehicle);
	}
}
