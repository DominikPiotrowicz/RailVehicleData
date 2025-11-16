using RailVehicleData.Application.Dto;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Application.Interfaces
{
	/// <summary>
	/// Interfejs serwisu obsługującego logikę biznesową pojazdów szynowych
	/// </summary>
	public interface IVehicleService
	{
		/// <summary>
		/// Pobiera wszystkie pojazdy
		/// </summary>
		/// <returns>Lista DTO wszystkich pojazdów</returns>
		Task<IEnumerable<VehicleDto>> GetAllVehicleAsync();

		/// <summary>
		/// Pobiera pojazd o określonym identyfikatorze
		/// </summary>
		/// <param name="id">Identyfikator pojazdu</param>
		/// <returns>DTO pojazdu lub null jeśli nie znaleziono</returns>
		Task<VehicleDto?> GetVehicleByIdAsync(int id);

		/// <summary>
		/// Dodaje nowy pojazd określonego typu
		/// </summary>
		/// <typeparam name="T">Typ pojazdu (Vehicle lub ElectricVehicle)</typeparam>
		/// <param name="newVehicleDto">DTO z danymi nowego pojazdu</param>
		/// <returns>Utworzony pojazd</returns>
		Vehicle AddNew<T>(NewVehicleDto newVehicleDto) where T : Vehicle;

		/// <summary>
		/// Aktualizuje istniejący pojazd
		/// </summary>
		/// <param name="id">Identyfikator pojazdu</param>
		/// <param name="vehicleDto">Zaktualizowane dane pojazdu</param>
		Task UpdateVehicleAsync(int id, NewVehicleDto vehicleDto);

		/// <summary>
		/// Usuwa pojazd o określonym identyfikatorze
		/// </summary>
		/// <param name="id">Identyfikator pojazdu do usunięcia</param>
		Task DeleteVehicleAsync(int id);
	}
}
