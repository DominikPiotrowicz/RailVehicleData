using Microsoft.EntityFrameworkCore;
using RailVehicleData.Aplication;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Aplication.Mappings;
using RailVehicleData.Domain.Interfaces;
using RailVehicleData.Infrastrcture.Data;
using RailVehicleData.Infrastrcture.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddSingleton(AutoMapperConfig.Initialize());

builder.Services.AddDbContext<RailVehicleDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("RailVehicleCS")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
