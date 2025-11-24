using Microsoft.EntityFrameworkCore;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.ValueObjects;

namespace RailVehicleData.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for RailVehicleData.
/// Uses Clean Architecture patterns with proper configuration for aggregates and value objects.
/// </summary>
public class RailVehicleDbContext : DbContext
{
    public RailVehicleDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Vehicles (Aggregate Root).
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; }

    /// <summary>
    /// Traction Systems (TPH inheritance - single table for all types).
    /// </summary>
    public DbSet<TractionSystem> TractionSystems { get; set; }

    /// <summary>
    /// Multiple Units (Aggregate Root for EMU/DMU).
    /// </summary>
    public DbSet<MultipleUnit> MultipleUnits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Vehicle aggregate
        ConfigureVehicle(modelBuilder);

        // Configure TractionSystem (TPH inheritance)
        ConfigureTractionSystem(modelBuilder);

        // Configure MultipleUnit aggregate
        ConfigureMultipleUnit(modelBuilder);
    }

    /// <summary>
    /// Configures Vehicle entity and its owned CommonSpecifications value object.
    /// </summary>
    private void ConfigureVehicle(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Vehicle>();

        entity.HasKey(v => v.VehicleId);

        entity.Property(v => v.VehicleId)
            .ValueGeneratedNever();

        entity.Property(v => v.Manufacturer)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(v => v.ManufacturedYear)
            .IsRequired();

        entity.Property(v => v.Role)
            .IsRequired()
            .HasConversion<string>();

        // Own CommonSpecifications value object
        entity.OwnsOne(v => v.CommonSpecifications, nav =>
        {
            // Configure Length properties
            nav.Property(cs => cs.Length)
                .HasColumnName("CommonSpecifications_Length_Millimeters")
                .HasConversion(
                    v => v.Millimeters,
                    d => Length.FromMillimeters(d));

            // Configure Width properties
            nav.Property(cs => cs.Width)
                .HasColumnName("CommonSpecifications_Width_Millimeters")
                .HasConversion(
                    v => v.Millimeters,
                    d => Length.FromMillimeters(d));

            // Configure Height properties
            nav.Property(cs => cs.Height)
                .HasColumnName("CommonSpecifications_Height_Millimeters")
                .HasConversion(
                    v => v.Millimeters,
                    d => Length.FromMillimeters(d));

            // Configure WheelDiameter properties
            nav.Property(cs => cs.WheelDiameter)
                .HasColumnName("CommonSpecifications_WheelDiameter_Millimeters")
                .HasConversion(
                    v => v.Millimeters,
                    d => Length.FromMillimeters(d));

            // Configure AxleCount
            nav.Property(cs => cs.AxleCount)
                .HasColumnName("CommonSpecifications_AxleCount");

            // Configure ServiceWeight properties
            nav.Property(cs => cs.ServiceWeight)
                .HasColumnName("CommonSpecifications_ServiceWeight_Kilograms")
                .HasConversion(
                    v => v.Kilograms,
                    d => Weight.FromKilograms(d));

            // Configure MaxSpeed properties
            nav.Property(cs => cs.MaxSpeed)
                .HasColumnName("CommonSpecifications_MaxSpeed_KmH")
                .HasConversion(
                    v => v.KilometersPerHour,
                    d => Speed.FromKilometersPerHour(d));

            // Optional specifications
            nav.Property(cs => cs.NumberOfSeats)
                .HasColumnName("CommonSpecifications_NumberOfSeats");

            nav.Property(cs => cs.NumberOfDoors)
                .HasColumnName("CommonSpecifications_NumberOfDoors");

            nav.Property(cs => cs.ToiletCapacity)
                .HasColumnName("CommonSpecifications_ToiletCapacity_Liters")
                .HasConversion(
                    v => v == null ? (decimal?)null : v.Liters,
                    d => d.HasValue ? Volume.FromLiters(d.Value) : null);

            nav.Property(cs => cs.HasClimateControl)
                .HasColumnName("CommonSpecifications_HasClimateControl");
        });

        // Foreign key for MultipleUnit
        entity.HasOne<MultipleUnit>()
            .WithMany()
            .HasForeignKey(v => v.MultipleUnitId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Foreign key to Series (if needed in future)
        entity.Property(v => v.SeriesId).IsRequired(false);

        // Relationship to TractionSystems
        entity.HasMany<TractionSystem>()
            .WithOne()
            .HasForeignKey(ts => ts.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(v => v.Manufacturer);
        entity.HasIndex(v => v.Model);
        entity.HasIndex(v => v.ManufacturedYear);
        entity.HasIndex(v => v.MultipleUnitId);
    }

    /// <summary>
    /// Configures TractionSystem (TPH - Table Per Hierarchy).
    /// All traction types in single table with Discriminator column.
    /// </summary>
    private void ConfigureTractionSystem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TractionSystem>();

        entity.HasKey(ts => ts.TractionSystemId);

        entity.Property(ts => ts.TractionSystemId)
            .ValueGeneratedNever();

        entity.Property(ts => ts.VehicleId)
            .IsRequired();

        entity.Property(ts => ts.InstalledDate)
            .IsRequired();

        entity.Property(ts => ts.RemovedDate)
            .IsRequired(false);

        // MaxPower Value Object conversion
        entity.Property(ts => ts.MaxPower)
            .HasColumnName("MaxPower_Kilowatts")
            .HasConversion(
                v => v == null ? (int?)null : v.Kilowatts,
                d => d.HasValue ? new Power(d.Value) : null);

        // TPH Discriminator configuration
        entity.HasDiscriminator<string>("TractionSystemType")
            .HasValue<ElectricTraction>("Electric")
            .HasValue<DieselTraction>("Diesel")
            .HasValue<SteamTraction>("Steam");

        // Configure ElectricTraction
        modelBuilder.Entity<ElectricTraction>()
            .Property(et => et.VoltageSystem)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<ElectricTraction>()
            .Property(et => et.PantographType)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<ElectricTraction>()
            .Property(et => et.ACFrequency)
            .HasColumnName("ACFrequency_Hertz")
            .HasConversion(
                v => v == null ? (int?)null : v.Hertz,
                d => d.HasValue ? new Frequency(d.Value) : null);

        modelBuilder.Entity<ElectricTraction>()
            .Property(et => et.HasRegenerativeBraking);

        // Configure DieselTraction
        modelBuilder.Entity<DieselTraction>()
            .Property(dt => dt.FuelCapacity)
            .HasColumnName("FuelCapacity_Liters")
            .HasConversion(
                v => v.Liters,
                d => Volume.FromLiters(d));

        modelBuilder.Entity<DieselTraction>()
            .Property(dt => dt.EngineType)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<DieselTraction>()
            .Property(dt => dt.EmissionStandard)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<DieselTraction>()
            .Property(dt => dt.CylinderCount)
            .IsRequired();

        modelBuilder.Entity<DieselTraction>()
            .Property(dt => dt.FuelConsumptionPer100Km)
            .IsRequired(false);

        // Configure SteamTraction
        modelBuilder.Entity<SteamTraction>()
            .Property(st => st.BoilerWorkingPressure)
            .HasColumnName("BoilerWorkingPressure_Bars")
            .HasConversion(
                v => v.Bars,
                d => new Pressure(d));

        modelBuilder.Entity<SteamTraction>()
            .Property(st => st.BoilerMaxPressure)
            .HasColumnName("BoilerMaxPressure_Bars")
            .HasConversion(
                v => v.Bars,
                d => new Pressure(d));

        modelBuilder.Entity<SteamTraction>()
            .Property(st => st.BoilerCapacity)
            .HasColumnName("BoilerCapacity_Liters")
            .HasConversion(
                v => v.Liters,
                d => Volume.FromLiters(d));

        modelBuilder.Entity<SteamTraction>()
            .Property(st => st.GrateArea)
            .HasColumnName("GrateArea_SquareMeters")
            .HasConversion(
                v => v.SquareMeters,
                d => Area.FromSquareMeters(d));

        modelBuilder.Entity<SteamTraction>()
            .Property(st => st.FireboxType)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<SteamTraction>()
            .Property(st => st.FuelType)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<SteamTraction>()
            .Property(st => st.HeatingArea)
            .HasColumnName("HeatingArea_SquareMeters")
            .HasConversion(
                v => v == null ? (decimal?)null : v.SquareMeters,
                d => d.HasValue ? Area.FromSquareMeters(d.Value) : null);

        // Indexes
        entity.HasIndex(ts => ts.VehicleId);
        entity.HasIndex(ts => ts.InstalledDate);
        entity.HasIndex(ts => ts.RemovedDate);
    }

    /// <summary>
    /// Configures MultipleUnit aggregate.
    /// </summary>
    private void ConfigureMultipleUnit(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<MultipleUnit>();

        entity.HasKey(mu => mu.MultipleUnitId);

        entity.Property(mu => mu.MultipleUnitId)
            .ValueGeneratedNever();

        entity.Property(mu => mu.Designation)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(mu => mu.CarCount)
            .IsRequired();

        entity.Property(mu => mu.FirstCarNumber)
            .IsRequired();

        entity.Property(mu => mu.LastCarNumber)
            .IsRequired();

        entity.Property(mu => mu.TotalCapacity)
            .IsRequired();

        entity.Property(mu => mu.CommissionedDate)
            .IsRequired();

        entity.Property(mu => mu.DecommissionedDate)
            .IsRequired(false);

        entity.Property(mu => mu.Type)
            .IsRequired()
            .HasConversion<string>();

        // Value Object conversions
        entity.Property(mu => mu.TotalLength)
            .HasColumnName("TotalLength_Millimeters")
            .HasConversion(
                v => v.Millimeters,
                d => Length.FromMillimeters(d));

        entity.Property(mu => mu.MaxSpeed)
            .HasColumnName("MaxSpeed_KmH")
            .HasConversion(
                v => v.KilometersPerHour,
                d => Speed.FromKilometersPerHour(d));

        // Indexes
        entity.HasIndex(mu => mu.Designation).IsUnique();
        entity.HasIndex(mu => mu.Type);
        entity.HasIndex(mu => mu.CommissionedDate);
    }
}

