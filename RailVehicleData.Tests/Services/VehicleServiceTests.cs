using AutoMapper;
using FluentAssertions;
using Moq;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Mappings;
using RailVehicleData.Application.Services;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Domain.Interfaces;
using Xunit;

namespace RailVehicleData.Tests.Services
{
	/// <summary>
	/// Testy jednostkowe dla VehicleService
	/// </summary>
	public class VehicleServiceTests
	{
		private readonly Mock<IVehicleRepository> _mockRepository;
		private readonly IMapper _mapper;
		private readonly VehicleService _service;

		public VehicleServiceTests()
		{
			_mockRepository = new Mock<IVehicleRepository>();
			_mapper = AutoMapperConfig.Initialize();
			_service = new VehicleService(_mockRepository.Object, _mapper);
		}

		#region GetAllVehicleAsync Tests

		[Fact]
		public async Task GetAllVehicleAsync_ShouldReturnAllVehicles_WhenVehiclesExist()
		{
			// Arrange
			var vehicles = new List<Vehicle>
			{
				new Vehicle { VehicleId = 1, Model = "SM42", Manufacturer = "Fablok" },
				new ElectricVehicle { VehicleId = 2, Model = "EU07", Manufacturer = "Pafawag", Voltage = "3000V" }
			};

			_mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(vehicles);

			// Act
			var result = await _service.GetAllVehicleAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(2);
			result.First().Model.Should().Be("SM42");
			_mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
		}

		[Fact]
		public async Task GetAllVehicleAsync_ShouldReturnEmptyList_WhenNoVehiclesExist()
		{
			// Arrange
			_mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Vehicle>());

			// Act
			var result = await _service.GetAllVehicleAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeEmpty();
			_mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
		}

		#endregion

		#region GetVehicleByIdAsync Tests

		[Fact]
		public async Task GetVehicleByIdAsync_ShouldReturnVehicle_WhenVehicleExists()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975
			};

			_mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);

			// Act
			var result = await _service.GetVehicleByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result!.VehicleId.Should().Be(1);
			result.Model.Should().Be("SM42");
			result.Manufacturer.Should().Be("Fablok");
			_mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
		}

		[Fact]
		public async Task GetVehicleByIdAsync_ShouldReturnNull_WhenVehicleDoesNotExist()
		{
			// Arrange
			_mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Vehicle?)null);

			// Act
			var result = await _service.GetVehicleByIdAsync(999);

			// Assert
			result.Should().BeNull();
			_mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
		}

		[Fact]
		public async Task GetVehicleByIdAsync_ShouldThrowArgumentException_WhenIdIsZero()
		{
			// Act
			Func<Task> act = async () => await _service.GetVehicleByIdAsync(0);

			// Assert
			await act.Should().ThrowAsync<ArgumentException>()
				.WithMessage("*ID pojazdu musi być większe od 0*");
		}

		[Fact]
		public async Task GetVehicleByIdAsync_ShouldThrowArgumentException_WhenIdIsNegative()
		{
			// Act
			Func<Task> act = async () => await _service.GetVehicleByIdAsync(-1);

			// Assert
			await act.Should().ThrowAsync<ArgumentException>()
				.WithMessage("*ID pojazdu musi być większe od 0*");
		}

		#endregion

		#region AddNew Tests

		[Fact]
		public void AddNew_ShouldAddVehicle_WhenValidDtoProvided()
		{
			// Arrange
			var newVehicleDto = new NewVehicleDto
			{
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975,
				MaxSpeed = 90,
				Weight = 82
			};

			var addedVehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok"
			};

			_mockRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>())).ReturnsAsync(addedVehicle);
			_mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

			// Act
			var result = _service.AddNew<Vehicle>(newVehicleDto);

			// Assert
			result.Should().NotBeNull();
			result.VehicleId.Should().Be(1);
			result.Model.Should().Be("SM42");
			_mockRepository.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
			_mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public void AddNew_ShouldAddElectricVehicle_WhenValidElectricDtoProvided()
		{
			// Arrange
			var newVehicleDto = new NewVehicleDto
			{
				Model = "EU07",
				Manufacturer = "Pafawag",
				ProductionYear = 1965,
				Voltage = "3000V",
				CurrentType = "DC",
				EnginePower = 2000
			};

			var addedVehicle = new ElectricVehicle
			{
				VehicleId = 1,
				Model = "EU07",
				Voltage = "3000V"
			};

			_mockRepository.Setup(r => r.AddAsync(It.IsAny<ElectricVehicle>())).ReturnsAsync(addedVehicle);
			_mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

			// Act
			var result = _service.AddNew<ElectricVehicle>(newVehicleDto);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<ElectricVehicle>();
			result.VehicleId.Should().Be(1);
			result.Model.Should().Be("EU07");
			_mockRepository.Verify(r => r.AddAsync(It.IsAny<ElectricVehicle>()), Times.Once);
		}

		[Fact]
		public void AddNew_ShouldThrowArgumentNullException_WhenDtoIsNull()
		{
			// Act
			Action act = () => _service.AddNew<Vehicle>(null!);

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParameterName("newVehicleDto");
		}

		#endregion

		#region UpdateVehicleAsync Tests

		[Fact]
		public async Task UpdateVehicleAsync_ShouldUpdateVehicle_WhenVehicleExists()
		{
			// Arrange
			var existingVehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok"
			};

			var updateDto = new NewVehicleDto
			{
				Model = "SM42-Updated",
				Manufacturer = "Fablok Updated",
				ProductionYear = 1980
			};

			_mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingVehicle);
			_mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);
			_mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

			// Act
			await _service.UpdateVehicleAsync(1, updateDto);

			// Assert
			_mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
			_mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Once);
			_mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task UpdateVehicleAsync_ShouldThrowKeyNotFoundException_WhenVehicleDoesNotExist()
		{
			// Arrange
			var updateDto = new NewVehicleDto { Model = "Test" };
			_mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Vehicle?)null);

			// Act
			Func<Task> act = async () => await _service.UpdateVehicleAsync(999, updateDto);

			// Assert
			await act.Should().ThrowAsync<KeyNotFoundException>()
				.WithMessage("*999*");
		}

		[Fact]
		public async Task UpdateVehicleAsync_ShouldThrowArgumentException_WhenIdIsInvalid()
		{
			// Arrange
			var updateDto = new NewVehicleDto { Model = "Test" };

			// Act
			Func<Task> act = async () => await _service.UpdateVehicleAsync(0, updateDto);

			// Assert
			await act.Should().ThrowAsync<ArgumentException>()
				.WithMessage("*ID pojazdu musi być większe od 0*");
		}

		[Fact]
		public async Task UpdateVehicleAsync_ShouldThrowArgumentNullException_WhenDtoIsNull()
		{
			// Act
			Func<Task> act = async () => await _service.UpdateVehicleAsync(1, null!);

			// Assert
			await act.Should().ThrowAsync<ArgumentNullException>()
				.WithParameterName("vehicleDto");
		}

		#endregion

		#region DeleteVehicleAsync Tests

		[Fact]
		public async Task DeleteVehicleAsync_ShouldDeleteVehicle_WhenVehicleExists()
		{
			// Arrange
			var vehicle = new Vehicle { VehicleId = 1, Model = "SM42" };
			_mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);
			_mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
			_mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

			// Act
			await _service.DeleteVehicleAsync(1);

			// Assert
			_mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
			_mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
			_mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task DeleteVehicleAsync_ShouldThrowKeyNotFoundException_WhenVehicleDoesNotExist()
		{
			// Arrange
			_mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Vehicle?)null);

			// Act
			Func<Task> act = async () => await _service.DeleteVehicleAsync(999);

			// Assert
			await act.Should().ThrowAsync<KeyNotFoundException>()
				.WithMessage("*999*");
		}

		[Fact]
		public async Task DeleteVehicleAsync_ShouldThrowArgumentException_WhenIdIsInvalid()
		{
			// Act
			Func<Task> act = async () => await _service.DeleteVehicleAsync(0);

			// Assert
			await act.Should().ThrowAsync<ArgumentException>()
				.WithMessage("*ID pojazdu musi być większe od 0*");
		}

		#endregion

		#region Constructor Tests

		[Fact]
		public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
		{
			// Act
			Action act = () => new VehicleService(null!, _mapper);

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParameterName("vehicleRepository");
		}

		[Fact]
		public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
		{
			// Act
			Action act = () => new VehicleService(_mockRepository.Object, null!);

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParameterName("mapper");
		}

		#endregion
	}
}
