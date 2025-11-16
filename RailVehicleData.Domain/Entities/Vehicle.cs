namespace RailVehicleData.Domain.Entities
{
	/// <summary>
	/// Bazowa encja reprezentująca pojazd szynowy
	/// </summary>
	public class Vehicle
	{
		/// <summary>
		/// Unikalny identyfikator pojazdu
		/// </summary>
		public int VehicleId { get; set; }

		/// <summary>
		/// Model pojazdu
		/// </summary>
		public string? Model { get; set; }

		/// <summary>
		/// Nazwa producenta pojazdu
		/// </summary>
		public string? Manufacturer { get; set; }

		/// <summary>
		/// Rok produkcji pojazdu
		/// </summary>
		public int? ProductionYear { get; set; }

		/// <summary>
		/// Maksymalna prędkość w km/h
		/// </summary>
		public int? MaxSpeed { get; set; }

		/// <summary>
		/// Masa pojazdu w tonach
		/// </summary>
		public decimal? Weight { get; set; }

		/// <summary>
		/// Długość pojazdu w metrach
		/// </summary>
		public decimal? Length { get; set; }

		/// <summary>
		/// Szerokość pojazdu w metrach
		/// </summary>
		public decimal? Width { get; set; }

		/// <summary>
		/// Wysokość pojazdu w metrach
		/// </summary>
		public decimal? Height { get; set; }

		/// <summary>
		/// Rodzaj napędu (np. elektryczny, spalinowy, parowy)
		/// </summary>
		public string? PowerType { get; set; }

		/// <summary>
		/// Data utworzenia rekordu
		/// </summary>
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Data ostatniej modyfikacji rekordu
		/// </summary>
		public DateTime? ModifiedDate { get; set; }
	}
}
