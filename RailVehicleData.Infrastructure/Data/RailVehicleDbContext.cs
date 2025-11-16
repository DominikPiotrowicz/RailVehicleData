using Microsoft.EntityFrameworkCore;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Infrastructure.Data
{
	/// <summary>
	/// Kontekst bazy danych Entity Framework dla pojazdów szynowych
	/// </summary>
	public class RailVehicleDbContext : DbContext
	{
		public RailVehicleDbContext(DbContextOptions<RailVehicleDbContext> options)
			: base(options)
		{
		}

		/// <summary>
		/// Tabela pojazdów szynowych
		/// </summary>
		public DbSet<Vehicle> Vehicles { get; set; } = null!;

		/// <summary>
		/// Tabela pojazdów elektrycznych
		/// </summary>
		public DbSet<ElectricVehicle> ElectricVehicles { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Konfiguracja encji Vehicle
			modelBuilder.Entity<Vehicle>(entity =>
			{
				entity.HasKey(e => e.VehicleId);

				entity.Property(e => e.Model)
					.HasMaxLength(100);

				entity.Property(e => e.Manufacturer)
					.HasMaxLength(100);

				entity.Property(e => e.PowerType)
					.HasMaxLength(50);

				entity.Property(e => e.Weight)
					.HasPrecision(10, 2);

				entity.Property(e => e.Length)
					.HasPrecision(10, 2);

				entity.Property(e => e.Width)
					.HasPrecision(10, 2);

				entity.Property(e => e.Height)
					.HasPrecision(10, 2);

				entity.Property(e => e.CreatedDate)
					.HasDefaultValueSql("GETUTCDATE()");

				// Konfiguracja TPH (Table Per Hierarchy) - wszystkie pojazdy w jednej tabeli
				entity.HasDiscriminator<string>("VehicleType")
					.HasValue<Vehicle>("Standard")
					.HasValue<ElectricVehicle>("Electric");
			});

			// Konfiguracja encji ElectricVehicle
			modelBuilder.Entity<ElectricVehicle>(entity =>
			{
				entity.Property(e => e.Voltage)
					.HasMaxLength(50);

				entity.Property(e => e.CurrentType)
					.HasMaxLength(10);

				entity.Property(e => e.EnginePower)
					.HasPrecision(10, 2);

				entity.Property(e => e.MotorType)
					.HasMaxLength(100);

				entity.Property(e => e.WheelArrangement)
					.HasMaxLength(20);

				entity.Property(e => e.MaxTractiveEffort)
					.HasPrecision(10, 2);
			});

			// Dane początkowe (seed data)
			SeedData(modelBuilder);
		}

		/// <summary>
		/// Wypełnia bazę danych przykładowymi danymi
		/// </summary>
		private void SeedData(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Vehicle>().HasData(
				new Vehicle
				{
					VehicleId = 1,
					Model = "SM42",
					Manufacturer = "Fablok",
					ProductionYear = 1975,
					MaxSpeed = 90,
					Weight = 82,
					Length = 14.7m,
					Width = 3.06m,
					Height = 4.3m,
					PowerType = "Spalinowy",
					CreatedDate = DateTime.UtcNow
				},
				new Vehicle
				{
					VehicleId = 2,
					Model = "SP32",
					Manufacturer = "Fablok",
					ProductionYear = 1969,
					MaxSpeed = 80,
					Weight = 54,
					Length = 10.8m,
					Width = 3.06m,
					Height = 4.15m,
					PowerType = "Spalinowy",
					CreatedDate = DateTime.UtcNow
				}
			);

			modelBuilder.Entity<ElectricVehicle>().HasData(
				new ElectricVehicle
				{
					VehicleId = 3,
					Model = "EU07",
					Manufacturer = "Pafawag",
					ProductionYear = 1965,
					MaxSpeed = 125,
					Weight = 84,
					Length = 16.7m,
					Width = 3.08m,
					Height = 4.34m,
					PowerType = "Elektryczny",
					Voltage = "3000V",
					CurrentType = "DC",
					EnginePower = 2000,
					NumberOfMotors = 4,
					MotorType = "Prądu stałego",
					WheelArrangement = "Bo'Bo'",
					IsMultiSystem = false,
					MaxTractiveEffort = 240,
					CreatedDate = DateTime.UtcNow
				},
				new ElectricVehicle
				{
					VehicleId = 4,
					Model = "EP09",
					Manufacturer = "Pafawag",
					ProductionYear = 1986,
					MaxSpeed = 160,
					Weight = 84,
					Length = 18.2m,
					Width = 3.08m,
					Height = 4.32m,
					PowerType = "Elektryczny",
					Voltage = "3000V",
					CurrentType = "DC",
					EnginePower = 3000,
					NumberOfMotors = 4,
					MotorType = "Prądu stałego",
					WheelArrangement = "Bo'Bo'",
					IsMultiSystem = false,
					MaxTractiveEffort = 300,
					CreatedDate = DateTime.UtcNow
				}
			);
		}
	}
}
