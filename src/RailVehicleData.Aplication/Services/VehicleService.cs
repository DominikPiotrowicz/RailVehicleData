using AutoMapper;
using RailVehicleData.Aplication.Dto;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Interfaces;

namespace RailVehicleData.Aplication
{

	public class VehicleService : IVehicleService
	{
		private readonly IVehicleRepository _vehicleRepository;
		private readonly IMapper _mapper;

		public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
		{
			_vehicleRepository = vehicleRepository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<VehicleDto>> GetAllVehicleAsync()
		{
			var vehicles = await _vehicleRepository.GetAllVehiclesAsync();

			var vehicleDtos = _mapper.Map<List<VehicleDto>>(vehicles);

			return vehicleDtos;
		}

		public async Task<VehicleDto> GetVehicleByIdAsync(int id)
		{
			var vehicle = await _vehicleRepository.GetByIdAsync(id);
			var getvehicle = _mapper.Map<VehicleDto>(vehicle);
			return getvehicle;
		}

		public VehicleDto AddNew<T>(NewVehicleDto newVehicle) where T : Vehicle
		{

			var vehicle = _mapper.Map<Vehicle>(newVehicle);
			//var vehicleType = typeof(T).Name;
/*
			vehicle.TechnicalData = new TechnicalData()
			{
				Length = newVehicle.Length,
				Width = newVehicle.Width,
				Height = newVehicle.Height,
				ServiceWeight = newVehicle.ServiceWeight,
				WheelDiameter = newVehicle.WheelDiameter
			};

			vehicle.Series = new Series()
			{
				Name = newVehicle.SeriesName
			};*/


			_vehicleRepository.Add(vehicle);

			return _mapper.Map<VehicleDto>(vehicle);
		}
	}
}