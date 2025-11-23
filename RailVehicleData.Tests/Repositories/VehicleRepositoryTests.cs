using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Infrastructure.Data;
using RailVehicleData.Infrastructure.Repositories;
using Xunit;

namespace RailVehicleData.Tests.Repositories
{
	/// <summary>
	/// Testy jednostkowe dla VehicleRepository
	/// </summary>
	public class VehicleRepositoryTests : IDisposable
	{
		private readonly RailVehicleDbContext _context;
		private readonly VehicleRepository _repository;

		public VehicleRepositoryTests()
		{
			var options = new DbContextOptionsBuilder<RailVehicleDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new RailVehicleDbContext(options);
			_repository = new VehicleRepository(_context);
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		#region GetAllAsync Tests

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllVehicles_WhenVehiclesExist()
		{
			// Arrange
			var vehicles = new List<Vehicle>
			{
				new Vehicle { VehicleId = 1, Model = "SM42", Manufacturer = "Fablok" },
				new Vehicle { VehicleId = 2, Model = "SP32", Manufacturer = "Fablok" },
				new ElectricVehicle { VehicleId = 3, Model = "EU07", Manufacturer = "Pafawag", Voltage = "3000V" }
			};

			await _context.Vehicles.AddRangeAsync(vehicles);
			await _context.SaveChangesAsync();

			// Act
			var result = await _repository.GetAllAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(3);
			result.Should().Contain(v => v.Model == "SM42");
			result.Should().Contain(v => v.Model == "EU07");
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoVehiclesExist()
		{
			// Act
			var result = await _repository.GetAllAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnDetachedEntities()
		{
			// Arrange
			var vehicle = new Vehicle { VehicleId = 1, Model = "SM42" };
			await _context.Vehicles.AddAsync(vehicle);
			await _context.SaveChangesAsync();

			// Act
			var result = await _repository.GetAllAsync();
			var firstVehicle = result.First();

			// Assert
			_context.Entry(firstVehicle).State.Should().Be(EntityState.Detached);
		}

		#endregion

		#region GetByIdAsync Tests

		[Fact]
		public async Task GetByIdAsync_ShouldReturnVehicle_WhenVehicleExists()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975
			};

			await _context.Vehicles.AddAsync(vehicle);
			await _context.SaveChangesAsync();

			// Act
			var result = await _repository.GetByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result!.VehicleId.Should().Be(1);
			result.Model.Should().Be("SM42");
			result.Manufacturer.Should().Be("Fablok");
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnNull_WhenVehicleDoesNotExist()
		{
			// Act
			var result = await _repository.GetByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnElectricVehicle_WhenElectricVehicleExists()
		{
			// Arrange
			var electricVehicle = new ElectricVehicle
			{
				VehicleId = 1,
				Model = "EU07",
				Manufacturer = "Pafawag",
				Voltage = "3000V",
				CurrentType = "DC"
			};

			await _context.Vehicles.AddAsync(electricVehicle);
			await _context.SaveChangesAsync();

			// Act
			var result = await _repository.GetByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<ElectricVehicle>();
			var electricResult = result as ElectricVehicle;
			electricResult!.Voltage.Should().Be("3000V");
		}

		#endregion

		#region AddAsync Tests

		[Fact]
		public async Task AddAsync_ShouldAddVehicle_WhenValidVehicleProvided()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975
			};

			// Act
			var result = await _repository.AddAsync(vehicle);
			await _repository.SaveChangesAsync();

			// Assert
			result.Should().NotBeNull();
			result.VehicleId.Should().BeGreaterThan(0);

			var savedVehicle = await _context.Vehicles.FindAsync(result.VehicleId);
			savedVehicle.Should().NotBeNull();
			savedVehicle!.Model.Should().Be("SM42");
		}

		[Fact]
		public async Task AddAsync_ShouldAddElectricVehicle_WhenValidElectricVehicleProvided()
		{
			// Arrange
			var electricVehicle = new ElectricVehicle
			{
				Model = "EU07",
				Manufacturer = "Pafawag",
				Voltage = "3000V",
				CurrentType = "DC",
				EnginePower = 2000
			};

			// Act
			var result = await _repository.AddAsync(electricVehicle);
			await _repository.SaveChangesAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<ElectricVehicle>();
			result.VehicleId.Should().BeGreaterThan(0);

			var savedVehicle = await _context.Vehicles.FindAsync(result.VehicleId);
			savedVehicle.Should().BeOfType<ElectricVehicle>();
		}

		[Fact]
		public async Task AddAsync_ShouldThrowArgumentNullException_WhenVehicleIsNull()
		{
			// Act
			Func<Task> act = async () => await _repository.AddAsync(null!);

			// Assert
			await act.Should().ThrowAsync<ArgumentNullException>();
		}

		#endregion

		#region UpdateAsync Tests

		[Fact]
		public async Task UpdateAsync_ShouldUpdateVehicle_WhenVehicleExists()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975
			};

			await _context.Vehicles.AddAsync(vehicle);
			await _context.SaveChangesAsync();

			// Detach to simulate getting from another context
			_context.Entry(vehicle).State = EntityState.Detached;

			vehicle.Model = "SM42-Updated";
			vehicle.ProductionYear = 1980;

			// Act
			await _repository.UpdateAsync(vehicle);
			await _repository.SaveChangesAsync();

			// Assert
			var updatedVehicle = await _context.Vehicles.FindAsync(vehicle.VehicleId);
			updatedVehicle.Should().NotBeNull();
			updatedVehicle!.Model.Should().Be("SM42-Updated");
			updatedVehicle.ProductionYear.Should().Be(1980);
		}

		[Fact]
		public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenVehicleIsNull()
		{
			// Act
			Func<Task> act = async () => await _repository.UpdateAsync(null!);

			// Assert
			await act.Should().ThrowAsync<ArgumentNullException>();
		}

		#endregion

		#region DeleteAsync Tests

		[Fact]
		public async Task DeleteAsync_ShouldDeleteVehicle_WhenVehicleExists()
		{
			// Arrange
			var vehicle = new Vehicle
			{
				Model = "SM42",
				Manufacturer = "Fablok"
			};

			await _context.Vehicles.AddAsync(vehicle);
			await _context.SaveChangesAsync();
			var vehicleId = vehicle.VehicleId;

			// Act
			await _repository.DeleteAsync(vehicleId);
			await _repository.SaveChangesAsync();

			// Assert
			var deletedVehicle = await _context.Vehicles.FindAsync(vehicleId);
			deletedVehicle.Should().BeNull();
		}

		[Fact]
		public async Task DeleteAsync_ShouldDoNothing_WhenVehicleDoesNotExist()
		{
			// Arrange
			var initialCount = await _context.Vehicles.CountAsync();

			// Act
			await _repository.DeleteAsync(999);
			await _repository.SaveChangesAsync();

			// Assert
			var finalCount = await _context.Vehicles.CountAsync();
			finalCount.Should().Be(initialCount);
		}

		#endregion

		#region SaveChangesAsync Tests

		[Fact]
		public async Task SaveChangesAsync_ShouldPersistChanges()
		{
			// Arrange
			var vehicle = new Vehicle { Model = "SM42", Manufacturer = "Fablok" };
			await _repository.AddAsync(vehicle);

			// Act
			await _repository.SaveChangesAsync();

			// Assert
			var savedVehicle = await _context.Vehicles.FirstOrDefaultAsync();
			savedVehicle.Should().NotBeNull();
			savedVehicle!.Model.Should().Be("SM42");
		}

		#endregion

		#region Constructor Tests

		[Fact]
		public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
		{
			// Act
			Action act = () => new VehicleRepository(null!);

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParameterName("context");
		}

		#endregion
	}
}
