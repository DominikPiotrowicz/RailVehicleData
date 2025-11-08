using RailVehicleData.Domain.Entities;
using RailVehicleData.Infrastrcture.Data;

namespace RailVehicleData.Infrastrcture.Seeders
{
	public class VehicleSeeder
	{
		private readonly RailVehicleDbContext _context;

		public VehicleSeeder(RailVehicleDbContext context)
		{
			_context = context;
		}

		public void Seed()
		{
			if (_context.Database.CanConnect())
			{
				if (!_context.ElectricVehicles.Any())
				{
					var electricVehicles = GetElectricVehicles();
					_context.ElectricVehicles.AddRange(electricVehicles);
					_context.SaveChanges();
				}
			}
		}

		public static IEnumerable<ElectricVehicle> GetElectricVehicles()
		{
			var electricvehicles = new List<ElectricVehicle>()
			{
				new ElectricVehicle()
				{
					Manufacturer = "PaFaWag",
					Model = "4E",
					Year = 1964,
					/*Series = new Series()
					{
						Name = "EU07"
					}*/
				}
			};
			return electricvehicles;
		}
	}
}
