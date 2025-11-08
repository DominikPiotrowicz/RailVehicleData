using Microsoft.EntityFrameworkCore;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Infrastrcture.Data
{
	public class RailVehicleDbContext : DbContext
	{
		public RailVehicleDbContext(DbContextOptions options) : base(options)
		{

		}
		public DbSet<Vehicle > Vehicles { get; set; } 
		public DbSet<ElectricVehicle> ElectricVehicles { get; set; }
		public DbSet<ElectricMultipleUnit> ElectricMultipleUnits { get; set; }
		public DbSet<DieselVehicle> DieselVehicles { get; set; }
		public DbSet<DieselMultipleUnit> DieselMultipleUnits { get; set; }
		public DbSet<StreamVehicle> StreamVehicles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Vehicle>(entity =>
			{
				entity.HasOne(v => v.VehicleType)
				.WithMany(vt => vt.Vehicles)
				.HasForeignKey(vt => vt.VehicleTypeId);
			});


			modelBuilder.Entity<ElectricVehicle>(entity =>
			{
				entity.HasOne(ev => ev.Vehicle)
				.WithOne(v => v.ElectricVehicle)
				.HasForeignKey<ElectricVehicle>(ev => ev.VehicleId);
			});

		}
	}
}

