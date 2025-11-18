using AutoMapper;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Exceptions;
using RailVehicleData.Domain.ValueObjects;

namespace RailVehicleData.Application.Services;

/// <summary>
/// Service implementation for MultipleUnit (EMU/DMU) aggregate operations.
/// Orchestrates repository and mapping for multiple unit management.
/// Uses dependency inversion with IMultipleUnitRepository and IVehicleRepository interfaces.
/// </summary>
public class MultipleUnitService : IMultipleUnitService
{
    private readonly IMultipleUnitRepository _multipleUnitRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public MultipleUnitService(
        IMultipleUnitRepository multipleUnitRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        _multipleUnitRepository = multipleUnitRepository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MultipleUnitDto>> GetAllMultipleUnitsAsync()
    {
        var multipleUnits = await _multipleUnitRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<MultipleUnitDto>>(multipleUnits);
    }

    public async Task<MultipleUnitDto> GetMultipleUnitByIdAsync(Guid multipleUnitId)
    {
        var multipleUnit = await _multipleUnitRepository.GetByIdAsync(multipleUnitId);
        return _mapper.Map<MultipleUnitDto>(multipleUnit);
    }

    public async Task<MultipleUnitDto> GetMultipleUnitByDesignationAsync(string designation)
    {
        var multipleUnit = await _multipleUnitRepository.GetByDesignationAsync(designation);
        return _mapper.Map<MultipleUnitDto>(multipleUnit);
    }

    public async Task<IEnumerable<MultipleUnitDto>> GetMultipleUnitsByTypeAsync(string type)
    {
        if (!Enum.TryParse<MultipleUnitType>(type, ignoreCase: true, out var multipleUnitType))
            throw new DomainException($"Invalid multiple unit type: {type}. Valid types: Electric, Diesel, Hybrid");

        var multipleUnits = await _multipleUnitRepository.GetByTypeAsync(multipleUnitType);
        return _mapper.Map<IEnumerable<MultipleUnitDto>>(multipleUnits);
    }

    public async Task<IEnumerable<MultipleUnitDto>> GetActiveMultipleUnitsAsync()
    {
        var multipleUnits = await _multipleUnitRepository.GetActiveAsync();
        return _mapper.Map<IEnumerable<MultipleUnitDto>>(multipleUnits);
    }

    public async Task<IEnumerable<MultipleUnitDto>> GetDecommissionedMultipleUnitsAsync()
    {
        var multipleUnits = await _multipleUnitRepository.GetDecommissionedAsync();
        return _mapper.Map<IEnumerable<MultipleUnitDto>>(multipleUnits);
    }

    public async Task<IEnumerable<MultipleUnitDto>> GetIncompleteMultipleUnitsAsync()
    {
        var multipleUnits = await _multipleUnitRepository.GetIncompleteAsync();
        return _mapper.Map<IEnumerable<MultipleUnitDto>>(multipleUnits);
    }

    public async Task<MultipleUnitDto> CreateMultipleUnitAsync(NewMultipleUnitDto multipleUnitDto)
    {
        if (!Enum.TryParse<MultipleUnitType>(multipleUnitDto.Type, ignoreCase: true, out var type))
            throw new DomainException($"Invalid multiple unit type: {multipleUnitDto.Type}");

        var multipleUnit = MultipleUnit.Create(
            designation: multipleUnitDto.Designation,
            carCount: multipleUnitDto.CarCount,
            firstCarNumber: multipleUnitDto.FirstCarNumber,
            lastCarNumber: multipleUnitDto.LastCarNumber,
            totalCapacity: multipleUnitDto.TotalCapacity,
            totalLength: Length.FromMeters(multipleUnitDto.TotalLengthMeters),
            maxSpeed: Speed.FromKilometersPerHour(multipleUnitDto.MaxSpeedKmH),
            type: type,
            commissionedDate: multipleUnitDto.CommissionedDate
        );

        await _multipleUnitRepository.AddAsync(multipleUnit);
        return _mapper.Map<MultipleUnitDto>(multipleUnit);
    }

    public async Task<MultipleUnitDto> UpdateMultipleUnitAsync(Guid multipleUnitId, NewMultipleUnitDto multipleUnitDto)
    {
        var multipleUnit = await _multipleUnitRepository.GetByIdAsync(multipleUnitId);

        // Note: Most properties are immutable in domain model
        // This is a limitation we can improve later with proper update methods
        throw new NotImplementedException("MultipleUnit update not yet implemented due to immutable domain model design.");
    }

    public async Task<MultipleUnitDto> DecommissionMultipleUnitAsync(Guid multipleUnitId, DateTime decommissionDate)
    {
        var multipleUnit = await _multipleUnitRepository.GetByIdAsync(multipleUnitId);
        multipleUnit.Decommission(decommissionDate);
        await _multipleUnitRepository.UpdateAsync(multipleUnit);
        return _mapper.Map<MultipleUnitDto>(multipleUnit);
    }

    public async Task<MultipleUnitDto> AddCarToMultipleUnitAsync(Guid multipleUnitId, Guid vehicleId)
    {
        var multipleUnit = await _multipleUnitRepository.GetByIdAsync(multipleUnitId);
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        // Validate vehicle is not already in a multiple unit
        if (vehicle.MultipleUnitId.HasValue && vehicle.MultipleUnitId != multipleUnitId)
            throw new DomainException($"Vehicle {vehicleId} is already part of another multiple unit.");

        multipleUnit.AddCar(vehicleId);
        await _multipleUnitRepository.UpdateAsync(multipleUnit);
        return _mapper.Map<MultipleUnitDto>(multipleUnit);
    }

    public async Task<MultipleUnitDto> RemoveCarFromMultipleUnitAsync(Guid multipleUnitId, Guid vehicleId)
    {
        var multipleUnit = await _multipleUnitRepository.GetByIdAsync(multipleUnitId);
        multipleUnit.RemoveCar(vehicleId);
        await _multipleUnitRepository.UpdateAsync(multipleUnit);
        return _mapper.Map<MultipleUnitDto>(multipleUnit);
    }

    public async Task<IEnumerable<VehicleDto>> GetCarsInMultipleUnitAsync(Guid multipleUnitId)
    {
        var vehicles = await _vehicleRepository.GetByMultipleUnitAsync(multipleUnitId);
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task DeleteMultipleUnitAsync(Guid multipleUnitId)
    {
        await _multipleUnitRepository.DeleteAsync(multipleUnitId);
    }
}
