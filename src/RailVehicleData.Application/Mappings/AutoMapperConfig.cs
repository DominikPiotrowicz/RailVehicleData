using AutoMapper;
using RailVehicleData.Application.Dto;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.ValueObjects;

namespace RailVehicleData.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between Domain entities and DTOs.
/// </summary>
public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        // Vehicle mappings
        CreateMap<Vehicle, VehicleDto>()
            .ForMember(dest => dest.ManufacturedYear, opt => opt.MapFrom(src => src.ManufacturedYear))
            .ForMember(dest => dest.LengthMeters, opt => opt.MapFrom(src => src.CommonSpecifications.Length.ToMeters))
            .ForMember(dest => dest.WidthMeters, opt => opt.MapFrom(src => src.CommonSpecifications.Width.ToMeters))
            .ForMember(dest => dest.HeightMeters, opt => opt.MapFrom(src => src.CommonSpecifications.Height.ToMeters))
            .ForMember(dest => dest.WheelDiameterMeters, opt => opt.MapFrom(src => src.CommonSpecifications.WheelDiameter.ToMeters))
            .ForMember(dest => dest.AxleCount, opt => opt.MapFrom(src => src.CommonSpecifications.AxleCount))
            .ForMember(dest => dest.ServiceWeightTons, opt => opt.MapFrom(src => src.CommonSpecifications.ServiceWeight.ToTons))
            .ForMember(dest => dest.MaxSpeedKmH, opt => opt.MapFrom(src => src.CommonSpecifications.MaxSpeed.ToKilometersPerHour))
            .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => src.CommonSpecifications.NumberOfSeats))
            .ForMember(dest => dest.NumberOfDoors, opt => opt.MapFrom(src => src.CommonSpecifications.NumberOfDoors))
            .ForMember(dest => dest.ToiletCapacityLiters, opt => opt.MapFrom(src => src.CommonSpecifications.ToiletCapacity != null ? src.CommonSpecifications.ToiletCapacity.Liters : (decimal?)null))
            .ForMember(dest => dest.HasClimateControl, opt => opt.MapFrom(src => src.CommonSpecifications.HasClimateControl))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.TractionSystems, opt => opt.MapFrom(src => src.TractionSystems))
            .ForMember(dest => dest.TotalActivePowerKw, opt => opt.MapFrom(src => src.TotalActivePower != null ? src.TotalActivePower.Kilowatts : (decimal?)null));

        // TractionSystem mappings
        CreateMap<TractionSystem, TractionSystemDto>()
            .Include<ElectricTraction, ElectricTractionDto>()
            .Include<DieselTraction, DieselTractionDto>()
            .Include<SteamTraction, SteamTractionDto>()
            .ForMember(dest => dest.MaxPowerKw, opt => opt.MapFrom(src => src.MaxPower != null ? src.MaxPower.Kilowatts : (int?)null))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.GetType().Name));

        // Electric Traction mapping
        CreateMap<ElectricTraction, ElectricTractionDto>()
            .IncludeBase<TractionSystem, TractionSystemDto>()
            .ForMember(dest => dest.ACFrequencyHz, opt => opt.MapFrom(src => src.ACFrequency != null ? src.ACFrequency.Hertz : (int?)null));

        // Diesel Traction mapping
        CreateMap<DieselTraction, DieselTractionDto>()
            .IncludeBase<TractionSystem, TractionSystemDto>()
            .ForMember(dest => dest.FuelCapacityLiters, opt => opt.MapFrom(src => src.FuelCapacity.Liters));

        // Steam Traction mapping
        CreateMap<SteamTraction, SteamTractionDto>()
            .IncludeBase<TractionSystem, TractionSystemDto>()
            .ForMember(dest => dest.BoilerWorkingPressureBars, opt => opt.MapFrom(src => src.BoilerWorkingPressure.Bars))
            .ForMember(dest => dest.BoilerMaxPressureBars, opt => opt.MapFrom(src => src.BoilerMaxPressure.Bars))
            .ForMember(dest => dest.BoilerCapacityLiters, opt => opt.MapFrom(src => src.BoilerCapacity.Liters))
            .ForMember(dest => dest.GrateAreaSquareMeters, opt => opt.MapFrom(src => src.GrateArea.SquareMeters))
            .ForMember(dest => dest.HeatingAreaSquareMeters, opt => opt.MapFrom(src => src.HeatingArea != null ? src.HeatingArea.SquareMeters : (decimal?)null));

        // MultipleUnit mappings
        CreateMap<MultipleUnit, MultipleUnitDto>()
            .ForMember(dest => dest.TotalLengthMeters, opt => opt.MapFrom(src => src.TotalLength.ToMeters))
            .ForMember(dest => dest.MaxSpeedKmH, opt => opt.MapFrom(src => src.MaxSpeed.ToKilometersPerHour))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.CarIdsCount, opt => opt.MapFrom(src => src.CarIds.Count));
    }

    /// <summary>
    /// Factory method for creating mapper instance.
    /// </summary>
    public static IMapper Initialize()
    {
        return new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperConfig>();
        }).CreateMapper();
    }
}
