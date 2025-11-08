using AutoMapper;
using RailVehicleData.Aplication.Dto;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Exceptions;
using RailVehicleData.Domain.ValueObjects;

namespace RailVehicleData.Aplication.Services;

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

    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<VehicleDto> GetVehicleByIdAsync(Guid vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<IEnumerable<VehicleDto>> GetVehiclesByManufacturerAsync(string manufacturer)
    {
        var vehicles = await _vehicleRepository.GetByManufacturerAsync(manufacturer);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<IEnumerable<VehicleDto>> GetVehiclesByModelAsync(string model)
    {
        var vehicles = await _vehicleRepository.GetByModelAsync(model);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<IEnumerable<VehicleDto>> GetVehiclesByYearAsync(int year)
    {
        var vehicles = await _vehicleRepository.GetByManufacturedYearAsync(year);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<IEnumerable<VehicleDto>> GetStandaloneVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetStandaloneVehiclesAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<IEnumerable<VehicleDto>> GetActiveVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetActiveVehiclesAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<IEnumerable<VehicleDto>> GetVehiclesByMultipleUnitAsync(Guid multipleUnitId)
    {
        var vehicles = await _vehicleRepository.GetByMultipleUnitAsync(multipleUnitId);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<IEnumerable<TractionSystemDto>> GetVehicleTractionSystemsAsync(Guid vehicleId)
    {
        var tractionSystems = await _vehicleRepository.GetVehicleTractionSystemsAsync(vehicleId);
        return _mapper.Map<IEnumerable<TractionSystemDto>>(tractionSystems);
    }

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
        return _mapper.Map<VehicleDto>(vehicle);
    }

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
        return _mapper.Map<VehicleDto>(wagon);
    }

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

    public async Task<VehicleDto> DecommissionVehicleAsync(Guid vehicleId, DateTime decommissionDate)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        vehicle.Decommission(decommissionDate);
        await _vehicleRepository.UpdateAsync(vehicle);
        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<VehicleDto> AddTractionSystemAsync(Guid vehicleId, NewTractionSystemDto tractionSystemDto)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        var tractionSystem = CreateTractionSystemFromDto(tractionSystemDto, vehicleId);
        vehicle.AddTractionSystem(tractionSystem);
        await _vehicleRepository.UpdateAsync(vehicle);
        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<VehicleDto> RemoveTractionSystemAsync(Guid vehicleId, Guid tractionSystemId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        vehicle.RemoveTractionSystem(tractionSystemId);
        await _vehicleRepository.UpdateAsync(vehicle);
        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<VehicleDto> DecommissionTractionSystemAsync(Guid vehicleId, Guid tractionSystemId, DateTime removalDate)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        var tractionSystem = vehicle.TractionSystems.FirstOrDefault(ts => ts.TractionSystemId == tractionSystemId);

        if (tractionSystem == null)
            throw new DomainException($"Traction system {tractionSystemId} not found on vehicle {vehicleId}.");

        tractionSystem.Decommission(removalDate);
        await _vehicleRepository.UpdateAsync(vehicle);
        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task DeleteVehicleAsync(Guid vehicleId)
    {
        await _vehicleRepository.DeleteAsync(vehicleId);
    }

    /// <summary>
    /// Factory method to create TractionSystem from DTO.
    /// Uses polymorphism to instantiate correct type.
    /// </summary>
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