using AutoMapper;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Exceptions;
using RailVehicleData.Domain.ValueObjects;

namespace RailVehicleData.Application.Services;

/// <summary>
/// Service implementation for Vehicle aggregate operations.
/// Orchestrates repository and mapping for vehicle management.
/// Uses dependency inversion with IVehicleRepository interface.
/// </summary>
public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all vehicles in the system.
    /// </summary>
    /// <returns>An enumerable collection of all vehicles with their specifications and traction systems mapped to DTOs.</returns>
    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <summary>
    /// Retrieves a specific vehicle by its ID.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <returns>The vehicle with its complete specifications and traction systems.</returns>
    /// <exception cref="DomainException">Thrown if no vehicle with the specified ID exists.</exception>
    public async Task<VehicleDto> GetVehicleByIdAsync(Guid vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        return _mapper.Map<VehicleDto>(vehicle);
    }

    /// <summary>
    /// Retrieves all vehicles manufactured by a specific manufacturer.
    /// </summary>
    /// <param name="manufacturer">The name of the manufacturer (e.g., "Bombardier", "Siemens").</param>
    /// <returns>An enumerable collection of vehicles from the specified manufacturer.</returns>
    public async Task<IEnumerable<VehicleDto>> GetVehiclesByManufacturerAsync(string manufacturer)
    {
        var vehicles = await _vehicleRepository.GetByManufacturerAsync(manufacturer);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <summary>
    /// Retrieves all vehicles of a specific model.
    /// </summary>
    /// <param name="model">The model designation (e.g., "EU07", "SP32").</param>
    /// <returns>An enumerable collection of vehicles with the specified model.</returns>
    public async Task<IEnumerable<VehicleDto>> GetVehiclesByModelAsync(string model)
    {
        var vehicles = await _vehicleRepository.GetByModelAsync(model);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <summary>
    /// Retrieves all vehicles manufactured in a specific year.
    /// </summary>
    /// <param name="year">The year of manufacture.</param>
    /// <returns>An enumerable collection of vehicles manufactured in the specified year.</returns>
    public async Task<IEnumerable<VehicleDto>> GetVehiclesByYearAsync(int year)
    {
        var vehicles = await _vehicleRepository.GetByManufacturedYearAsync(year);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <summary>
    /// Retrieves all standalone vehicles (locomotives and independent cars).
    /// </summary>
    /// <returns>An enumerable collection of standalone vehicles not part of any multiple unit.</returns>
    public async Task<IEnumerable<VehicleDto>> GetStandaloneVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetStandaloneVehiclesAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <summary>
    /// Retrieves all active vehicles (not decommissioned).
    /// </summary>
    /// <returns>An enumerable collection of vehicles currently in service.</returns>
    public async Task<IEnumerable<VehicleDto>> GetActiveVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetActiveVehiclesAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <summary>
    /// Retrieves all vehicles that are part of a specific multiple unit (train set).
    /// </summary>
    /// <param name="multipleUnitId">The unique identifier of the multiple unit.</param>
    /// <returns>An enumerable collection of vehicles (wagons and traction cars) in the specified multiple unit.</returns>
    public async Task<IEnumerable<VehicleDto>> GetVehiclesByMultipleUnitAsync(Guid multipleUnitId)
    {
        var vehicles = await _vehicleRepository.GetByMultipleUnitAsync(multipleUnitId);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <summary>
    /// Retrieves all traction systems installed on a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <returns>An enumerable collection of all traction systems (electric, diesel, steam) on the vehicle.</returns>
    public async Task<IEnumerable<TractionSystemDto>> GetVehicleTractionSystemsAsync(Guid vehicleId)
    {
        var tractionSystems = await _vehicleRepository.GetVehicleTractionSystemsAsync(vehicleId);
        return _mapper.Map<IEnumerable<TractionSystemDto>>(tractionSystems);
    }

    /// <summary>
    /// Creates a new standalone vehicle (locomotive, electric railcar) with its traction systems.
    /// </summary>
    /// <param name="vehicleDto">The data transfer object containing vehicle specifications and traction system details.</param>
    /// <returns>The newly created vehicle mapped to a DTO.</returns>
    /// <exception cref="DomainException">Thrown if vehicle data is invalid or traction systems cannot be created.</exception>
    /// <remarks>
    /// This operation:
    /// 1. Converts DTO measurements to Value Objects (meters to Length, tons to Weight, etc.)
    /// 2. Creates CommonSpecifications from the physical specs
    /// 3. Creates the Vehicle aggregate with the specified traction systems
    /// 4. Persists to database atomically
    /// </remarks>
    public async Task<VehicleDto> CreateStandaloneVehicleAsync(NewVehicleDto vehicleDto)
    {
        // Create CommonSpecifications
        var commonSpecs = CommonSpecifications.Create(
            length: Length.FromMeters(vehicleDto.LengthMeters),
            width: Length.FromMeters(vehicleDto.WidthMeters),
            height: Length.FromMeters(vehicleDto.HeightMeters),
            wheelDiameter: Length.FromMeters(vehicleDto.WheelDiameterMeters),
            axleCount: vehicleDto.AxleCount,
            serviceWeight: Weight.FromTons(vehicleDto.ServiceWeightTons),
            maxSpeed: Speed.FromKilometersPerHour(vehicleDto.MaxSpeedKmH),
            numberOfSeats: vehicleDto.NumberOfSeats,
            numberOfDoors: vehicleDto.NumberOfDoors,
            toiletCapacity: vehicleDto.ToiletCapacityLiters.HasValue ? Volume.FromLiters(vehicleDto.ToiletCapacityLiters.Value) : null,
            hasClimateControl: vehicleDto.HasClimateControl
        );

        // Create vehicle
        var vehicle = Vehicle.CreateStandalone(
            manufacturer: vehicleDto.Manufacturer,
            model: vehicleDto.Model,
            manufacturedYear: vehicleDto.ManufacturedYear,
            commonSpecifications: commonSpecs,
            commissionedDate: vehicleDto.CommissionedDate,
            seriesId: vehicleDto.SeriesId
        );

        // Add traction systems
        foreach (var tractionDto in vehicleDto.TractionSystems)
        {
            var tractionSystem = CreateTractionSystemFromDto(tractionDto, vehicle.VehicleId);
            vehicle.AddTractionSystem(tractionSystem);
        }

        await _vehicleRepository.AddAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return _mapper.Map<VehicleDto>(vehicle);
    }

    /// <summary>
    /// Creates a new wagon (unpowered car) as part of a multiple unit (train set).
    /// </summary>
    /// <param name="wagonDto">The data transfer object containing wagon specifications.</param>
    /// <param name="multipleUnitId">The unique identifier of the multiple unit this wagon belongs to.</param>
    /// <returns>The newly created wagon mapped to a DTO.</returns>
    /// <exception cref="DomainException">Thrown if wagon data is invalid.</exception>
    /// <remarks>
    /// Wagons do not have traction systems - they are powered by traction vehicles in the same multiple unit.
    /// </remarks>
    public async Task<VehicleDto> CreateWagonAsync(NewVehicleDto wagonDto, Guid multipleUnitId)
    {
        // Create CommonSpecifications
        var commonSpecs = CommonSpecifications.Create(
            length: Length.FromMeters(wagonDto.LengthMeters),
            width: Length.FromMeters(wagonDto.WidthMeters),
            height: Length.FromMeters(wagonDto.HeightMeters),
            wheelDiameter: Length.FromMeters(wagonDto.WheelDiameterMeters),
            axleCount: wagonDto.AxleCount,
            serviceWeight: Weight.FromTons(wagonDto.ServiceWeightTons),
            maxSpeed: Speed.FromKilometersPerHour(wagonDto.MaxSpeedKmH),
            numberOfSeats: wagonDto.NumberOfSeats,
            numberOfDoors: wagonDto.NumberOfDoors,
            toiletCapacity: wagonDto.ToiletCapacityLiters.HasValue ? Volume.FromLiters(wagonDto.ToiletCapacityLiters.Value) : null,
            hasClimateControl: wagonDto.HasClimateControl
        );

        // Create wagon (note: wagons don't have traction systems)
        var wagon = Vehicle.CreateWagon(
            manufacturer: wagonDto.Manufacturer,
            model: wagonDto.Model,
            manufacturedYear: wagonDto.ManufacturedYear,
            commonSpecifications: commonSpecs,
            commissionedDate: wagonDto.CommissionedDate,
            multipleUnitId: multipleUnitId
        );

        await _vehicleRepository.AddAsync(wagon);
        await _vehicleRepository.SaveChangesAsync();
        return _mapper.Map<VehicleDto>(wagon);
    }

    /// <summary>
    /// Updates an existing vehicle's specifications (currently only CommonSpecifications).
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle to update.</param>
    /// <param name="vehicleDto">The data transfer object with updated specifications.</param>
    /// <returns>The updated vehicle mapped to a DTO.</returns>
    /// <exception cref="NotImplementedException">Currently not implemented due to immutable domain model design. Use domain aggregate methods instead.</exception>
    public async Task<VehicleDto> UpdateVehicleAsync(Guid vehicleId, NewVehicleDto vehicleDto)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        // Update basic properties (manufacturer, model, year are immutable in domain - so we don't update)
        // Only CommonSpecifications can be updated for now
        var newCommonSpecs = CommonSpecifications.Create(
            length: Length.FromMeters(vehicleDto.LengthMeters),
            width: Length.FromMeters(vehicleDto.WidthMeters),
            height: Length.FromMeters(vehicleDto.HeightMeters),
            wheelDiameter: Length.FromMeters(vehicleDto.WheelDiameterMeters),
            axleCount: vehicleDto.AxleCount,
            serviceWeight: Weight.FromTons(vehicleDto.ServiceWeightTons),
            maxSpeed: Speed.FromKilometersPerHour(vehicleDto.MaxSpeedKmH),
            numberOfSeats: vehicleDto.NumberOfSeats,
            numberOfDoors: vehicleDto.NumberOfDoors,
            toiletCapacity: vehicleDto.ToiletCapacityLiters.HasValue ? Volume.FromLiters(vehicleDto.ToiletCapacityLiters.Value) : null,
            hasClimateControl: vehicleDto.HasClimateControl
        );

        // Use reflection to update (domain model currently doesn't have public setter)
        // This is a limitation we can improve later
        throw new NotImplementedException("Vehicle update not yet implemented due to immutable domain model design.");
    }

    /// <summary>
    /// Decommissions a vehicle, marking the end of its service life.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle to decommission.</param>
    /// <param name="decommissionDate">The date when the vehicle was taken out of service.</param>
    /// <returns>The decommissioned vehicle mapped to a DTO.</returns>
    /// <exception cref="DomainException">Thrown if vehicle not found or if decommission date is invalid.</exception>
    public async Task<VehicleDto> DecommissionVehicleAsync(Guid vehicleId, DateTime decommissionDate)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        vehicle.Decommission(decommissionDate);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return _mapper.Map<VehicleDto>(vehicle);
    }

    /// <summary>
    /// Adds a new traction system to an existing vehicle (e.g., during modernization).
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <param name="tractionSystemDto">The data transfer object containing traction system specifications.</param>
    /// <returns>The updated vehicle with the new traction system.</returns>
    /// <exception cref="DomainException">Thrown if vehicle not found, or if traction system cannot be added to this vehicle type.</exception>
    public async Task<VehicleDto> AddTractionSystemAsync(Guid vehicleId, NewTractionSystemDto tractionSystemDto)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        var tractionSystem = CreateTractionSystemFromDto(tractionSystemDto, vehicleId);
        vehicle.AddTractionSystem(tractionSystem);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return _mapper.Map<VehicleDto>(vehicle);
    }

    /// <summary>
    /// Removes a traction system from a vehicle.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <param name="tractionSystemId">The unique identifier of the traction system to remove.</param>
    /// <returns>The updated vehicle with the traction system removed.</returns>
    /// <exception cref="DomainException">Thrown if vehicle or traction system not found.</exception>
    public async Task<VehicleDto> RemoveTractionSystemAsync(Guid vehicleId, Guid tractionSystemId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        vehicle.RemoveTractionSystem(tractionSystemId);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return _mapper.Map<VehicleDto>(vehicle);
    }

    /// <summary>
    /// Decommissions a specific traction system on a vehicle (marks it as removed).
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <param name="tractionSystemId">The unique identifier of the traction system to decommission.</param>
    /// <param name="removalDate">The date when the traction system was removed.</param>
    /// <returns>The updated vehicle with the traction system marked as inactive.</returns>
    /// <exception cref="DomainException">Thrown if vehicle or traction system not found, or if removal date is invalid.</exception>
    public async Task<VehicleDto> DecommissionTractionSystemAsync(Guid vehicleId, Guid tractionSystemId, DateTime removalDate)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        var tractionSystem = vehicle.TractionSystems.FirstOrDefault(ts => ts.TractionSystemId == tractionSystemId);

        if (tractionSystem == null)
            throw new DomainException($"Traction system {tractionSystemId} not found on vehicle {vehicleId}.");

        tractionSystem.Decommission(removalDate);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return _mapper.Map<VehicleDto>(vehicle);
    }

    /// <summary>
    /// Permanently deletes a vehicle from the system.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle to delete.</param>
    /// <exception cref="DomainException">Thrown if vehicle not found.</exception>
    /// <remarks>This is a hard delete operation. Use Decommission for soft deletion (marking as out of service).</remarks>
    public async Task DeleteVehicleAsync(Guid vehicleId)
    {
        await _vehicleRepository.DeleteAsync(vehicleId);
        await _vehicleRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Factory method to create a TractionSystem from a request DTO.
    /// Uses pattern matching to instantiate the correct traction type (Electric, Diesel, or Steam).
    /// </summary>
    /// <param name="dto">The traction system data transfer object containing type-specific information.</param>
    /// <param name="vehicleId">The unique identifier of the vehicle this traction system belongs to.</param>
    /// <returns>A concrete TractionSystem instance (ElectricTraction, DieselTraction, or SteamTraction).</returns>
    /// <remarks>
    /// This factory method demonstrates polymorphic object creation using C# pattern matching.
    /// It converts DTO measurements to Value Objects (kW to Power, liters to Volume, etc.).
    /// </remarks>
    private TractionSystem CreateTractionSystemFromDto(NewTractionSystemDto dto, Guid vehicleId)
    {
        return dto switch
        {
            NewElectricTractionDto et => new ElectricTraction(
                vehicleId: vehicleId,
                installedDate: et.InstalledDate,
                maxPower: new Power(et.MaxPowerKw),
                voltageSystem: et.VoltageSystem,
                hasRegenerativeBraking: et.HasRegenerativeBraking,
                pantographType: et.PantographType,
                acFrequency: et.ACFrequencyHz.HasValue ? new Frequency(et.ACFrequencyHz.Value) : null
            ),
            NewDieselTractionDto dt => new DieselTraction(
                vehicleId: vehicleId,
                installedDate: dt.InstalledDate,
                maxPower: new Power(dt.MaxPowerKw),
                fuelCapacity: Volume.FromLiters(dt.FuelCapacityLiters),
                engineType: dt.EngineType,
                emissionStandard: dt.EmissionStandard,
                cylinderCount: dt.CylinderCount,
                fuelConsumptionPer100Km: dt.FuelConsumptionPer100Km
            ),
            NewSteamTractionDto st => new SteamTraction(
                vehicleId: vehicleId,
                installedDate: st.InstalledDate,
                boilerWorkingPressure: new Pressure(st.BoilerWorkingPressureBars),
                boilerMaxPressure: new Pressure(st.BoilerMaxPressureBars),
                boilerCapacity: Volume.FromLiters(st.BoilerCapacityLiters),
                grateArea: Area.FromSquareMeters(st.GrateAreaSquareMeters),
                fireboxType: st.FireboxType,
                fuelType: st.FuelType,
                heatingArea: st.HeatingAreaSquareMeters.HasValue ? Area.FromSquareMeters(st.HeatingAreaSquareMeters.Value) : null
            ),
            _ => throw new DomainException($"Unknown traction system type: {dto.GetType().Name}")
        };
    }
}