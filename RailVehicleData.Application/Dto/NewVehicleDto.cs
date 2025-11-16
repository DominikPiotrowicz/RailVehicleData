using System.ComponentModel.DataAnnotations;

namespace RailVehicleData.Application.Dto
{
	/// <summary>
	/// DTO dla tworzenia nowego pojazdu szynowego
	/// </summary>
	public class NewVehicleDto
	{
		/// <summary>
		/// Model pojazdu
		/// </summary>
		[Required(ErrorMessage = "Model pojazdu jest wymagany")]
		[StringLength(100, ErrorMessage = "Model nie może przekraczać 100 znaków")]
		public string Model { get; set; } = string.Empty;

		/// <summary>
		/// Nazwa producenta pojazdu
		/// </summary>
		[StringLength(100, ErrorMessage = "Nazwa producenta nie może przekraczać 100 znaków")]
		public string? Manufacturer { get; set; }

		/// <summary>
		/// Rok produkcji pojazdu
		/// </summary>
		[Range(1800, 2100, ErrorMessage = "Rok produkcji musi być między 1800 a 2100")]
		public int? ProductionYear { get; set; }

		/// <summary>
		/// Maksymalna prędkość w km/h
		/// </summary>
		[Range(0, 600, ErrorMessage = "Maksymalna prędkość musi być między 0 a 600 km/h")]
		public int? MaxSpeed { get; set; }

		/// <summary>
		/// Masa pojazdu w tonach
		/// </summary>
		[Range(0, 1000, ErrorMessage = "Masa musi być między 0 a 1000 ton")]
		public decimal? Weight { get; set; }

		/// <summary>
		/// Długość pojazdu w metrach
		/// </summary>
		[Range(0, 100, ErrorMessage = "Długość musi być między 0 a 100 metrów")]
		public decimal? Length { get; set; }

		/// <summary>
		/// Szerokość pojazdu w metrach
		/// </summary>
		[Range(0, 10, ErrorMessage = "Szerokość musi być między 0 a 10 metrów")]
		public decimal? Width { get; set; }

		/// <summary>
		/// Wysokość pojazdu w metrach
		/// </summary>
		[Range(0, 10, ErrorMessage = "Wysokość musi być między 0 a 10 metrów")]
		public decimal? Height { get; set; }

		/// <summary>
		/// Rodzaj napędu (np. elektryczny, spalinowy, parowy)
		/// </summary>
		[StringLength(50, ErrorMessage = "Rodzaj napędu nie może przekraczać 50 znaków")]
		public string? PowerType { get; set; }

		// Właściwości specyficzne dla pojazdu elektrycznego
		/// <summary>
		/// Napięcie zasilania w V (dla pojazdów elektrycznych)
		/// </summary>
		[StringLength(50, ErrorMessage = "Napięcie nie może przekraczać 50 znaków")]
		public string? Voltage { get; set; }

		/// <summary>
		/// Typ prądu (AC - zmienny, DC - stały)
		/// </summary>
		[StringLength(10, ErrorMessage = "Typ prądu nie może przekraczać 10 znaków")]
		public string? CurrentType { get; set; }

		/// <summary>
		/// Moc silników elektrycznych w kW
		/// </summary>
		[Range(0, 50000, ErrorMessage = "Moc silnika musi być między 0 a 50000 kW")]
		public decimal? EnginePower { get; set; }

		/// <summary>
		/// Liczba silników trakcyjnych
		/// </summary>
		[Range(0, 20, ErrorMessage = "Liczba silników musi być między 0 a 20")]
		public int? NumberOfMotors { get; set; }

		/// <summary>
		/// Typ silnika elektrycznego
		/// </summary>
		[StringLength(100, ErrorMessage = "Typ silnika nie może przekraczać 100 znaków")]
		public string? MotorType { get; set; }

		/// <summary>
		/// Układ osi
		/// </summary>
		[StringLength(20, ErrorMessage = "Układ osi nie może przekraczać 20 znaków")]
		public string? WheelArrangement { get; set; }

		/// <summary>
		/// Czy pojazd ma możliwość pracy wielosystemowej
		/// </summary>
		public bool IsMultiSystem { get; set; }

		/// <summary>
		/// Maksymalna siła pociągowa w kN
		/// </summary>
		[Range(0, 1000, ErrorMessage = "Siła pociągowa musi być między 0 a 1000 kN")]
		public decimal? MaxTractiveEffort { get; set; }
	}
}
