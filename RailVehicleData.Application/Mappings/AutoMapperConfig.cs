using AutoMapper;
using RailVehicleData.Application.Dto;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Application.Mappings
{
	/// <summary>
	/// Konfiguracja mapowań AutoMapper dla aplikacji
	/// </summary>
	public class AutoMapperConfig
	{
		/// <summary>
		/// Inicjalizuje i konfiguruje AutoMapper
		/// </summary>
		/// <returns>Skonfigurowany obiekt IMapper</returns>
		public static IMapper Initialize()
		{
			var config = new MapperConfiguration(cfg =>
			{
				// Mapowanie z NewVehicleDto na Vehicle
				cfg.CreateMap<NewVehicleDto, Vehicle>()
					.ForMember(dest => dest.VehicleId, opt => opt.Ignore())
					.ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
					.ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

				// Mapowanie z NewVehicleDto na ElectricVehicle
				cfg.CreateMap<NewVehicleDto, ElectricVehicle>()
					.ForMember(dest => dest.VehicleId, opt => opt.Ignore())
					.ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
					.ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

				// Mapowanie z Vehicle na VehicleDto
				cfg.CreateMap<Vehicle, VehicleDto>()
					.ForMember(dest => dest.Voltage, opt => opt.Ignore())
					.ForMember(dest => dest.CurrentType, opt => opt.Ignore())
					.ForMember(dest => dest.EnginePower, opt => opt.Ignore())
					.ForMember(dest => dest.NumberOfMotors, opt => opt.Ignore())
					.ForMember(dest => dest.MotorType, opt => opt.Ignore())
					.ForMember(dest => dest.WheelArrangement, opt => opt.Ignore())
					.ForMember(dest => dest.IsMultiSystem, opt => opt.Ignore())
					.ForMember(dest => dest.MaxTractiveEffort, opt => opt.Ignore());

				// Mapowanie z ElectricVehicle na VehicleDto
				cfg.CreateMap<ElectricVehicle, VehicleDto>();

				// Mapowanie dwukierunkowe dla aktualizacji
				cfg.CreateMap<NewVehicleDto, Vehicle>()
					.ForMember(dest => dest.VehicleId, opt => opt.Ignore())
					.ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

				cfg.CreateMap<NewVehicleDto, ElectricVehicle>()
					.ForMember(dest => dest.VehicleId, opt => opt.Ignore())
					.ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
			});

			return config.CreateMapper();
		}
	}
}
