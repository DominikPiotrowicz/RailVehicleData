using AutoMapper;
using FluentAssertions;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Mappings;
using RailVehicleData.Domain.Entities;
using Xunit;

namespace RailVehicleData.Tests.Mappings
{
	/// <summary>
	/// Testy jednostkowe dla konfiguracji AutoMapper
	/// </summary>
	public class AutoMapperConfigTests
	{
		private readonly IMapper _mapper;

		public AutoMapperConfigTests()
		{
			_mapper = AutoMapperConfig.Initialize();
		}

		#region Configuration Validation Tests

		[Fact]
		public void AutoMapperConfig_ShouldHaveValidConfiguration()
		{
			// Act & Assert
			_mapper.ConfigurationProvider.AssertConfigurationIsValid();
		}

		#endregion

		#region NewVehicleDto to Vehicle Mapping Tests

		[Fact]
		public void Map_ShouldMapNewVehicleDtoToVehicle_WhenValidDtoProvided()
		{
			// Arrange
			var dto = new NewVehicleDto
			{
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975,
				MaxSpeed = 90,
				Weight = 82.5m,
				Length = 14.7m,
				Width = 3.06m,
				Height = 4.3m,
				PowerType = "Spalinowy"
			};

			// Act
			var vehicle = _mapper.Map<Vehicle>(dto);

			// Assert
			vehicle.Should().NotBeNull();
			vehicle.Model.Should().Be("SM42");
			vehicle.Manufacturer.Should().Be("Fablok");
			vehicle.ProductionYear.Should().Be(1975);
			vehicle.MaxSpeed.Should().Be(90);
			vehicle.Weight.Should().Be(82.5m);
			vehicle.Length.Should().Be(14.7m);
			vehicle.Width.Should().Be(3.06m);
			vehicle.Height.Should().Be(4.3m);
			vehicle.PowerType.Should().Be("Spalinowy");
			vehicle.VehicleId.Should().Be(0); // Should be ignored
		}

		[Fact]
		public void Map_ShouldIgnoreVehicleId_WhenMappingNewVehicleDto()
		{
			// Arrange
			var dto = new NewVehicleDto { Model = "SM42" };

			// Act
			var vehicle = _mapper.Map<Vehicle>(dto);

			// Assert
			vehicle.VehicleId.Should().Be(0);
		}

		#endregion

		#region NewVehicleDto to ElectricVehicle Mapping Tests

		[Fact]
		public void Map_ShouldMapNewVehicleDtoToElectricVehicle_WhenValidDtoProvided()
		{
			// Arrange
			var dto = new NewVehicleDto
			{
				Model = "EU07",
				Manufacturer = "Pafawag",
				ProductionYear = 1965,
				MaxSpeed = 125,
				Weight = 84,
				PowerType = "Elektryczny",
				Voltage = "3000V",
				CurrentType = "DC",
				EnginePower = 2000,
				NumberOfMotors = 4,
				MotorType = "Prądu stałego",
				WheelArrangement = "Bo'Bo'",
				IsMultiSystem = false,
				MaxTractiveEffort = 240
			};

			// Act
			var vehicle = _mapper.Map<ElectricVehicle>(dto);

			// Assert
			vehicle.Should().NotBeNull();
			vehicle.Model.Should().Be("EU07");
			vehicle.Manufacturer.Should().Be("Pafawag");
			vehicle.Voltage.Should().Be("3000V");
			vehicle.CurrentType.Should().Be("DC");
			vehicle.EnginePower.Should().Be(2000);
			vehicle.NumberOfMotors.Should().Be(4);
			vehicle.MotorType.Should().Be("Prądu stałego");
			vehicle.WheelArrangement.Should().Be("Bo'Bo'");
			vehicle.IsMultiSystem.Should().BeFalse();
			vehicle.MaxTractiveEffort.Should().Be(240);
		}

		[Fact]
		public void Map_ShouldMapBaseProperties_WhenMappingToElectricVehicle()
		{
			// Arrange
			var dto = new NewVehicleDto
			{
				Model = "EU07",
				Manufacturer = "Pafawag",
				MaxSpeed = 125
			};

			// Act
			var vehicle = _mapper.Map<ElectricVehicle>(dto);

			// Assert
			vehicle.Model.Should().Be("EU07");
			vehicle.Manufacturer.Should().Be("Pafawag");
			vehicle.MaxSpeed.Should().Be(125);
		}

		#endregion

		#region Vehicle to VehicleDto Mapping Tests

		[Fact]
		public void Map_ShouldMapVehicleToVehicleDto_WhenValidVehicleProvided()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975,
				MaxSpeed = 90,
				Weight = 82.5m,
				Length = 14.7m,
				Width = 3.06m,
				Height = 4.3m,
				PowerType = "Spalinowy",
				CreatedDate = DateTime.UtcNow,
				ModifiedDate = DateTime.UtcNow
			};

			// Act
			var dto = _mapper.Map<VehicleDto>(vehicle);

			// Assert
			dto.Should().NotBeNull();
			dto.VehicleId.Should().Be(1);
			dto.Model.Should().Be("SM42");
			dto.Manufacturer.Should().Be("Fablok");
			dto.ProductionYear.Should().Be(1975);
			dto.MaxSpeed.Should().Be(90);
			dto.Weight.Should().Be(82.5m);
			dto.Length.Should().Be(14.7m);
			dto.Width.Should().Be(3.06m);
			dto.Height.Should().Be(4.3m);
			dto.PowerType.Should().Be("Spalinowy");
			dto.CreatedDate.Should().BeCloseTo(vehicle.CreatedDate, TimeSpan.FromSeconds(1));
		}

		[Fact]
		public void Map_ShouldNotMapElectricProperties_WhenMappingStandardVehicle()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok"
			};

			// Act
			var dto = _mapper.Map<VehicleDto>(vehicle);

			// Assert
			dto.Voltage.Should().BeNull();
			dto.CurrentType.Should().BeNull();
			dto.EnginePower.Should().BeNull();
			dto.NumberOfMotors.Should().BeNull();
			dto.MotorType.Should().BeNull();
			dto.WheelArrangement.Should().BeNull();
			dto.IsMultiSystem.Should().BeNull();
			dto.MaxTractiveEffort.Should().BeNull();
		}

		#endregion

		#region ElectricVehicle to VehicleDto Mapping Tests

		[Fact]
		public void Map_ShouldMapElectricVehicleToVehicleDto_WhenValidElectricVehicleProvided()
		{
			// Arrange
			var electricVehicle = new ElectricVehicle
			{
				VehicleId = 1,
				Model = "EU07",
				Manufacturer = "Pafawag",
				ProductionYear = 1965,
				MaxSpeed = 125,
				Voltage = "3000V",
				CurrentType = "DC",
				EnginePower = 2000,
				NumberOfMotors = 4,
				MotorType = "Prądu stałego",
				WheelArrangement = "Bo'Bo'",
				IsMultiSystem = false,
				MaxTractiveEffort = 240,
				CreatedDate = DateTime.UtcNow
			};

			// Act
			var dto = _mapper.Map<VehicleDto>(electricVehicle);

			// Assert
			dto.Should().NotBeNull();
			dto.VehicleId.Should().Be(1);
			dto.Model.Should().Be("EU07");
			dto.Manufacturer.Should().Be("Pafawag");
			dto.Voltage.Should().Be("3000V");
			dto.CurrentType.Should().Be("DC");
			dto.EnginePower.Should().Be(2000);
			dto.NumberOfMotors.Should().Be(4);
			dto.MotorType.Should().Be("Prądu stałego");
			dto.WheelArrangement.Should().Be("Bo'Bo'");
			dto.IsMultiSystem.Should().BeFalse();
			dto.MaxTractiveEffort.Should().Be(240);
		}

		[Fact]
		public void Map_ShouldMapBothBaseAndElectricProperties_WhenMappingElectricVehicle()
		{
			// Arrange
			var electricVehicle = new ElectricVehicle
			{
				VehicleId = 1,
				Model = "EU07",
				MaxSpeed = 125,
				Voltage = "3000V"
			};

			// Act
			var dto = _mapper.Map<VehicleDto>(electricVehicle);

			// Assert
			dto.Model.Should().Be("EU07"); // Base property
			dto.MaxSpeed.Should().Be(125); // Base property
			dto.Voltage.Should().Be("3000V"); // Electric property
		}

		#endregion

		#region Collection Mapping Tests

		[Fact]
		public void Map_ShouldMapCollectionOfVehicles_WhenMultipleVehiclesProvided()
		{
			// Arrange
			var vehicles = new List<Vehicle>
			{
				new Vehicle { VehicleId = 1, Model = "SM42", Manufacturer = "Fablok" },
				new ElectricVehicle { VehicleId = 2, Model = "EU07", Manufacturer = "Pafawag", Voltage = "3000V" },
				new Vehicle { VehicleId = 3, Model = "SP32", Manufacturer = "Fablok" }
			};

			// Act
			var dtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);

			// Assert
			dtos.Should().NotBeNull();
			dtos.Should().HaveCount(3);
			dtos.First().Model.Should().Be("SM42");
			dtos.ElementAt(1).Voltage.Should().Be("3000V");
			dtos.Last().Model.Should().Be("SP32");
		}

		#endregion

		#region Null Handling Tests

		[Fact]
		public void Map_ShouldHandleNullProperties_WhenMappingDto()
		{
			// Arrange
			var dto = new NewVehicleDto
			{
				Model = "SM42",
				Manufacturer = null,
				ProductionYear = null
			};

			// Act
			var vehicle = _mapper.Map<Vehicle>(dto);

			// Assert
			vehicle.Model.Should().Be("SM42");
			vehicle.Manufacturer.Should().BeNull();
			vehicle.ProductionYear.Should().BeNull();
		}

		[Fact]
		public void Map_ShouldHandleNullProperties_WhenMappingEntity()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = null,
				ProductionYear = null
			};

			// Act
			var dto = _mapper.Map<VehicleDto>(vehicle);

			// Assert
			dto.Model.Should().Be("SM42");
			dto.Manufacturer.Should().BeNull();
			dto.ProductionYear.Should().BeNull();
		}

		#endregion

		#region Edge Cases Tests

		[Fact]
		public void Map_ShouldHandleMinimalDto_WhenOnlyRequiredFieldsProvided()
		{
			// Arrange
			var dto = new NewVehicleDto { Model = "Test" };

			// Act
			var vehicle = _mapper.Map<Vehicle>(dto);

			// Assert
			vehicle.Should().NotBeNull();
			vehicle.Model.Should().Be("Test");
		}

		[Fact]
		public void Map_ShouldHandleEmptyElectricProperties_WhenMappingToElectricVehicle()
		{
			// Arrange
			var dto = new NewVehicleDto
			{
				Model = "EU07",
				Voltage = null,
				CurrentType = null
			};

			// Act
			var vehicle = _mapper.Map<ElectricVehicle>(dto);

			// Assert
			vehicle.Model.Should().Be("EU07");
			vehicle.Voltage.Should().BeNull();
			vehicle.CurrentType.Should().BeNull();
		}

		#endregion
	}
}
