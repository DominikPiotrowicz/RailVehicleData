using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.ValueObjects;
using RailVehicleData.Infrastructure.Data;

namespace RailVehicleData.Infrastructure.Seeders;

/// <summary>
/// Database seeder for sample rail vehicle data.
/// Creates diverse examples of locomotives and multiple units.
/// </summary>
public class VehicleSeeder
{
    private readonly RailVehicleDbContext _context;

    public VehicleSeeder(RailVehicleDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seeds the database with sample vehicle and multiple unit data.
    /// </summary>
    public async Task SeedAsync()
    {
        if (_context.Database.CanConnect())
        {
            if (!_context.Vehicles.Any())
            {
                await SeedVehiclesAsync();
            }

            if (!_context.MultipleUnits.Any())
            {
                await SeedMultipleUnitsAsync();
            }
        }
    }

    /// <summary>
    /// Seeds standalone vehicles (locomotives).
    /// </summary>
    private async Task SeedVehiclesAsync()
    {
        var vehicles = new List<Vehicle>();

        // 1. Electric Locomotive (EU07 - Polish electric locomotive)
        var eu07 = Vehicle.CreateStandalone(
            manufacturer: "PaFaWag",
            model: "EU07",
            manufacturedYear: 1972,
            commonSpecifications: CommonSpecifications.Create(
                length: Length.FromMeters(19.9m),
                width: Length.FromMeters(2.99m),
                height: Length.FromMeters(4.3m),
                wheelDiameter: Length.FromMeters(1.25m),
                axleCount: 4,
                serviceWeight: Weight.FromTons(80),
                maxSpeed: Speed.FromKilometersPerHour(160)
            ),
            commissionedDate: new DateTime(1972, 1, 15)
        );
        eu07.AddTractionSystem(new ElectricTraction(
            vehicleId: eu07.VehicleId,
            installedDate: new DateTime(1972, 1, 15),
            maxPower: new Power(3200),
            voltageSystem: "3 kV DC",
            hasRegenerativeBraking: false,
            pantographType: "Standard DSS"
        ));
        vehicles.Add(eu07);

        // 2. Diesel Locomotive (SP32 - Polish diesel locomotive)
        var sp32 = Vehicle.CreateStandalone(
            manufacturer: "FABLOK",
            model: "SP32",
            manufacturedYear: 1965,
            commonSpecifications: CommonSpecifications.Create(
                length: Length.FromMeters(18.6m),
                width: Length.FromMeters(2.99m),
                height: Length.FromMeters(4.1m),
                wheelDiameter: Length.FromMeters(1.1m),
                axleCount: 4,
                serviceWeight: Weight.FromTons(70),
                maxSpeed: Speed.FromKilometersPerHour(100)
            ),
            commissionedDate: new DateTime(1965, 3, 20)
        );
        sp32.AddTractionSystem(new DieselTraction(
            vehicleId: sp32.VehicleId,
            installedDate: new DateTime(1965, 3, 20),
            maxPower: new Power(1200),
            fuelCapacity: Volume.FromLiters(3500),
            engineType: "6-cylinder Diesel",
            emissionStandard: "Euro 0",
            cylinderCount: 6
        ));
        vehicles.Add(sp32);

        // 3. Steam Locomotive (Ty2 - Polish narrow-gauge steam)
        var ty2 = Vehicle.CreateStandalone(
            manufacturer: "Chrzanów",
            model: "Ty2",
            manufacturedYear: 1952,
            commonSpecifications: CommonSpecifications.Create(
                length: Length.FromMeters(11.4m),
                width: Length.FromMeters(2.5m),
                height: Length.FromMeters(3.95m),
                wheelDiameter: Length.FromMeters(0.9m),
                axleCount: 4,
                serviceWeight: Weight.FromTons(28),
                maxSpeed: Speed.FromKilometersPerHour(60)
            ),
            commissionedDate: new DateTime(1952, 6, 10)
        );
        ty2.AddTractionSystem(new SteamTraction(
            vehicleId: ty2.VehicleId,
            installedDate: new DateTime(1952, 6, 10),
            boilerWorkingPressure: new Pressure(12),
            boilerMaxPressure: new Pressure(13),
            boilerCapacity: Volume.FromLiters(3000),
            grateArea: Area.FromSquareMeters(1.2m),
            fireboxType: "Belpaire",
            fuelType: "Coal",
            heatingArea: Area.FromSquareMeters(48)
        ));
        vehicles.Add(ty2);

        // 4. Modern Electric Locomotive (EP09 - Polish modern electric)
        var ep09 = Vehicle.CreateStandalone(
            manufacturer: "Pafawag",
            model: "EP09",
            manufacturedYear: 2019,
            commonSpecifications: CommonSpecifications.Create(
                length: Length.FromMeters(21.6m),
                width: Length.FromMeters(3.0m),
                height: Length.FromMeters(4.45m),
                wheelDiameter: Length.FromMeters(1.25m),
                axleCount: 4,
                serviceWeight: Weight.FromTons(86),
                maxSpeed: Speed.FromKilometersPerHour(200)
            ),
            commissionedDate: new DateTime(2019, 9, 1)
        );
        ep09.AddTractionSystem(new ElectricTraction(
            vehicleId: ep09.VehicleId,
            installedDate: new DateTime(2019, 9, 1),
            maxPower: new Power(6400),
            voltageSystem: "25 kV AC 50 Hz",
            hasRegenerativeBraking: true,
            pantographType: "Pantograph DSS230/025",
            acFrequency: new Frequency(50)
        ));
        vehicles.Add(ep09);

        await _context.Vehicles.AddRangeAsync(vehicles);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds Multiple Units (EMU/DMU).
    /// </summary>
    private async Task SeedMultipleUnitsAsync()
    {
        var multipleUnits = new List<MultipleUnit>();

        // 1. Electric Multiple Unit (EMU Class 395 - UK/Europe)
        var emu395 = MultipleUnit.Create(
            designation: "Class 395",
            carCount: 8,
            firstCarNumber: 395001,
            lastCarNumber: 395008,
            totalCapacity: 900,
            totalLength: Length.FromMeters(201),
            maxSpeed: Speed.FromKilometersPerHour(300),
            type: MultipleUnitType.Electric,
            commissionedDate: new DateTime(2009, 6, 1)
        );
        multipleUnits.Add(emu395);

        // 2. Diesel Multiple Unit (DMU Class 153 - UK Regional)
        var dmu153 = MultipleUnit.Create(
            designation: "Class 153",
            carCount: 2,
            firstCarNumber: 153301,
            lastCarNumber: 153302,
            totalCapacity: 120,
            totalLength: Length.FromMeters(52),
            maxSpeed: Speed.FromKilometersPerHour(145),
            type: MultipleUnitType.Diesel,
            commissionedDate: new DateTime(1992, 3, 15)
        );
        multipleUnits.Add(dmu153);

        // 3. Modern Hybrid Multiple Unit (EMU with diesel module)
        var hybridEmu = MultipleUnit.Create(
            designation: "Hybrid-220",
            carCount: 4,
            firstCarNumber: 220001,
            lastCarNumber: 220004,
            totalCapacity: 450,
            totalLength: Length.FromMeters(105),
            maxSpeed: Speed.FromKilometersPerHour(250),
            type: MultipleUnitType.Hybrid,
            commissionedDate: new DateTime(2021, 11, 1)
        );
        multipleUnits.Add(hybridEmu);

        await _context.MultipleUnits.AddRangeAsync(multipleUnits);
        await _context.SaveChangesAsync();
    }
}
