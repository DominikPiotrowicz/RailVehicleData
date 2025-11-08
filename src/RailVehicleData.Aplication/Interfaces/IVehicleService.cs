using RailVehicleData.Aplication.Dto;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Aplication.Interfaces
{
	public interface IVehicleService
	{
		Task<IEnumerable<VehicleDto>> GetAllVehicleAsync();
		Task<VehicleDto> GetVehicleByIdAsync(int id);
		VehicleDto AddNew<T>(NewVehicleDto newVehicle) where T : Vehicle;
	}
}
