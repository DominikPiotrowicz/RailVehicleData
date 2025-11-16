using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Domain.Interfaces
{
	/// <summary>
	/// Interfejs repozytorium dla operacji na pojazdach szynowych
	/// </summary>
	public interface IVehicleRepository
	{
		/// <summary>
		/// Pobiera wszystkie pojazdy z bazy danych
		/// </summary>
		/// <returns>Lista wszystkich pojazdów</returns>
		Task<IEnumerable<Vehicle>> GetAllAsync();

		/// <summary>
		/// Pobiera pojazd o określonym identyfikatorze
		/// </summary>
		/// <param name="id">Identyfikator pojazdu</param>
		/// <returns>Pojazd lub null jeśli nie znaleziono</returns>
		Task<Vehicle?> GetByIdAsync(int id);

		/// <summary>
		/// Dodaje nowy pojazd do bazy danych
		/// </summary>
		/// <param name="vehicle">Pojazd do dodania</param>
		/// <returns>Dodany pojazd z przypisanym ID</returns>
		Task<Vehicle> AddAsync(Vehicle vehicle);

		/// <summary>
		/// Aktualizuje istniejący pojazd
		/// </summary>
		/// <param name="vehicle">Pojazd do aktualizacji</param>
		Task UpdateAsync(Vehicle vehicle);

		/// <summary>
		/// Usuwa pojazd o określonym identyfikatorze
		/// </summary>
		/// <param name="id">Identyfikator pojazdu do usunięcia</param>
		Task DeleteAsync(int id);

		/// <summary>
		/// Zapisuje zmiany w bazie danych
		/// </summary>
		Task SaveChangesAsync();
	}
}
