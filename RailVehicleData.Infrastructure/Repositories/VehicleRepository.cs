using Microsoft.EntityFrameworkCore;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Interfaces;
using RailVehicleData.Infrastructure.Data;

namespace RailVehicleData.Infrastructure.Repositories
{
	/// <summary>
	/// Implementacja repozytorium dla operacji na pojazdach szynowych
	/// </summary>
	public class VehicleRepository : IVehicleRepository
	{
		private readonly RailVehicleDbContext _context;

		public VehicleRepository(RailVehicleDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Pobiera wszystkie pojazdy z bazy danych
		/// </summary>
		public async Task<IEnumerable<Vehicle>> GetAllAsync()
		{
			return await _context.Vehicles
				.AsNoTracking()
				.ToListAsync();
		}

		/// <summary>
		/// Pobiera pojazd o określonym identyfikatorze
		/// </summary>
		public async Task<Vehicle?> GetByIdAsync(int id)
		{
			return await _context.Vehicles
				.AsNoTracking()
				.FirstOrDefaultAsync(v => v.VehicleId == id);
		}

		/// <summary>
		/// Dodaje nowy pojazd do bazy danych
		/// </summary>
		public async Task<Vehicle> AddAsync(Vehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException(nameof(vehicle));
			}

			var entry = await _context.Vehicles.AddAsync(vehicle);
			return entry.Entity;
		}

		/// <summary>
		/// Aktualizuje istniejący pojazd
		/// </summary>
		public Task UpdateAsync(Vehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException(nameof(vehicle));
			}

			_context.Vehicles.Update(vehicle);
			return Task.CompletedTask;
		}

		/// <summary>
		/// Usuwa pojazd o określonym identyfikatorze
		/// </summary>
		public async Task DeleteAsync(int id)
		{
			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle != null)
			{
				_context.Vehicles.Remove(vehicle);
			}
		}

		/// <summary>
		/// Zapisuje zmiany w bazie danych
		/// </summary>
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
