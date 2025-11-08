using Microsoft.EntityFrameworkCore;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Interfaces;
using RailVehicleData.Infrastrcture.Data;

namespace RailVehicleData.Infrastrcture.Repositories
{
	public class VehicleRepository : IVehicleRepository
	{
		private readonly RailVehicleDbContext _dbContext;

		public VehicleRepository(RailVehicleDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Vehicle>> GetAllVehiclesAsync()
		{
			return await _dbContext.Vehicles
				//.Include(v => v.TechnicalData)
				//.Include(v => v.Series)
				.ToListAsync();
		}

		public async Task AddVehicleAsync(Vehicle vehicle)
		{
			await _dbContext.Vehicles.AddAsync(vehicle);
			await _dbContext.SaveChangesAsync();
		}

		public void Add(Vehicle vehicle)
		{
			//_dbContext.Vehicles.Add(vehicle);
			_dbContext.Set<Vehicle>().Add(vehicle);
			_dbContext.SaveChanges();
		}

		public async Task<Vehicle> GetByIdAsync(int id)
		{
			var vechicle = await _dbContext
				.Vehicles.FirstOrDefaultAsync(c => c.VehicleId == id);

			return vechicle == null ? throw new Exception("Series not found") : vechicle;
		}
	}
}
