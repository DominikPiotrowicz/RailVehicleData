using AutoMapper;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Interfaces;

namespace RailVehicleData.Application.Services
{
	/// <summary>
	/// Serwis obsługujący logikę biznesową pojazdów szynowych
	/// </summary>
	public class VehicleService : IVehicleService
	{
		private readonly IVehicleRepository _vehicleRepository;
		private readonly IMapper _mapper;

		public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
		{
			_vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		/// <summary>
		/// Pobiera wszystkie pojazdy z bazy danych
		/// </summary>
		public async Task<IEnumerable<VehicleDto>> GetAllVehicleAsync()
		{
			var vehicles = await _vehicleRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
		}

		/// <summary>
		/// Pobiera pojazd o określonym identyfikatorze
		/// </summary>
		public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentException("ID pojazdu musi być większe od 0", nameof(id));
			}

			var vehicle = await _vehicleRepository.GetByIdAsync(id);
			return vehicle != null ? _mapper.Map<VehicleDto>(vehicle) : null;
		}

		/// <summary>
		/// Dodaje nowy pojazd określonego typu
		/// </summary>
		public Vehicle AddNew<T>(NewVehicleDto newVehicleDto) where T : Vehicle
		{
			if (newVehicleDto == null)
			{
				throw new ArgumentNullException(nameof(newVehicleDto));
			}

			// Mapowanie DTO na encję odpowiedniego typu
			var vehicle = _mapper.Map<T>(newVehicleDto);
			vehicle.CreatedDate = DateTime.UtcNow;

			// Synchroniczne dodanie - w przyszłości można zmienić na async
			var addedVehicle = _vehicleRepository.AddAsync(vehicle).GetAwaiter().GetResult();
			_vehicleRepository.SaveChangesAsync().GetAwaiter().GetResult();

			return addedVehicle;
		}

		/// <summary>
		/// Aktualizuje istniejący pojazd
		/// </summary>
		public async Task UpdateVehicleAsync(int id, NewVehicleDto vehicleDto)
		{
			if (id <= 0)
			{
				throw new ArgumentException("ID pojazdu musi być większe od 0", nameof(id));
			}

			if (vehicleDto == null)
			{
				throw new ArgumentNullException(nameof(vehicleDto));
			}

			var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
			if (existingVehicle == null)
			{
				throw new KeyNotFoundException($"Pojazd o ID {id} nie został znaleziony");
			}

			// Mapowanie zaktualizowanych danych
			_mapper.Map(vehicleDto, existingVehicle);
			existingVehicle.ModifiedDate = DateTime.UtcNow;

			await _vehicleRepository.UpdateAsync(existingVehicle);
			await _vehicleRepository.SaveChangesAsync();
		}

		/// <summary>
		/// Usuwa pojazd o określonym identyfikatorze
		/// </summary>
		public async Task DeleteVehicleAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentException("ID pojazdu musi być większe od 0", nameof(id));
			}

			var vehicle = await _vehicleRepository.GetByIdAsync(id);
			if (vehicle == null)
			{
				throw new KeyNotFoundException($"Pojazd o ID {id} nie został znaleziony");
			}

			await _vehicleRepository.DeleteAsync(id);
			await _vehicleRepository.SaveChangesAsync();
		}
	}
}
