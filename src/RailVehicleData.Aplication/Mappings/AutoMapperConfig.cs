using AutoMapper;
using RailVehicleData.Aplication.Dto;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.Aplication.Mappings
{
    public class AutoMapperConfig : Profile
    {
        public static IMapper Initialize() => new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Vehicle, VehicleDto>();


            cfg.CreateMap<NewVehicleDto, Vehicle>();
        }).CreateMapper();
    }
}
