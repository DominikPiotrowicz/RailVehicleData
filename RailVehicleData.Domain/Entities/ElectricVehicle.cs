namespace RailVehicleData.Domain.Entities
{
	/// <summary>
	/// Encja reprezentująca pojazd elektryczny (lokomotywa elektryczna, elektrowóz)
	/// </summary>
	public class ElectricVehicle : Vehicle
	{
		/// <summary>
		/// Napięcie zasilania w V (np. 3000V DC, 15000V AC)
		/// </summary>
		public string? Voltage { get; set; }

		/// <summary>
		/// Typ prądu (AC - zmienny, DC - stały)
		/// </summary>
		public string? CurrentType { get; set; }

		/// <summary>
		/// Moc silników elektrycznych w kW
		/// </summary>
		public decimal? EnginePower { get; set; }

		/// <summary>
		/// Liczba silników trakcyjnych
		/// </summary>
		public int? NumberOfMotors { get; set; }

		/// <summary>
		/// Typ silnika elektrycznego (np. asynchroniczny, synchroniczny)
		/// </summary>
		public string? MotorType { get; set; }

		/// <summary>
		/// Układ osi (np. Bo'Bo', Co'Co')
		/// </summary>
		public string? WheelArrangement { get; set; }

		/// <summary>
		/// Czy pojazd ma możliwość pracy wielosystemowej
		/// </summary>
		public bool IsMultiSystem { get; set; }

		/// <summary>
		/// Maksymalna siła pociągowa w kN
		/// </summary>
		public decimal? MaxTractiveEffort { get; set; }
	}
}
