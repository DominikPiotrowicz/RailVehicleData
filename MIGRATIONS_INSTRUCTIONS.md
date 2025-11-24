# Entity Framework Core Migrations Guide

Ta instrukcja opisuje jak wygenerować migracje EF Core i utworzyć bazę danych dla projektu RailVehicleData.

## Wymagania

- ✅ .NET 6.0+ lub .NET 7.0 SDK
- ✅ SQL Server (LocalDB, Express, lub instancja)
- ✅ Entity Framework Core CLI tools

## Instalacja EF Core Tools (jeśli nie posiadasz)

```bash
dotnet tool install --global dotnet-ef
```

Lub aktualizacja:

```bash
dotnet tool update --global dotnet-ef
```

## Kroki Konfiguracji

### 1. Ustaw Connection String

Plik: `src/RailVehicleData.WebAPI/appsettings.json` (lub `appsettings.Development.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RailVehicleData;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### 2. Dodaj DbContext do Program.cs

Plik: `src/RailVehicleData.WebAPI/Program.cs`

```csharp
using RailVehicleData.Infrastructure.Data;
using RailVehicleData.Application.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<RailVehicleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add application services (repositories, services, mappers)
builder.Services.AddApplicationServices();

builder.Services.AddControllers();

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RailVehicleDbContext>();

    // Apply pending migrations
    dbContext.Database.Migrate();

    // Seed sample data
    var seeder = new RailVehicleData.Infrastructure.Seeders.VehicleSeeder(dbContext);
    await seeder.SeedAsync();
}

app.MapControllers();
app.Run();
```

### 3. Instalacja Wymaganych NuGet Packages

W każdym projekcie, jeśli brakuje:

```bash
# Domain
cd src/RailVehicleData.Domain
# (powinno być OK - tylko C#)

# Application
cd ../RailVehicleData.Application
dotnet add package AutoMapper --version 12.0.1

# Infrastructure
cd ../RailVehicleData.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.16

# WebAPI (projekt startowy)
cd ../RailVehicleData.WebAPI
dotnet add package Microsoft.EntityFrameworkCore --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.16
```

## Generowanie i Aplikowanie Migracji

### Krok 1: Dodaj Initial Migration

Z katalogu root projektu (gdzie jest RailVehicleData.sln):

```bash
dotnet ef migrations add InitialCreate \
  --project src/RailVehicleData.Infrastructure \
  --startup-project src/RailVehicleData.WebAPI
```

Powinno stworzyć folder `Migrations/` w `RailVehicleData.Infrastructure`.

### Krok 2: Zweryfikuj Migrację

Plik: `src/RailVehicleData.Infrastructure/Migrations/[timestamp]_InitialCreate.cs`

Sprawdź czy zawiera:
- ✅ Tabela `Vehicles`
- ✅ Tabela `TractionSystems` (z kolumną `TractionSystemType` - discriminator)
- ✅ Tabela `MultipleUnits`
- ✅ Indexes na klucze obcych i pola search
- ✅ Conversion Value Objects (Length, Weight, Power itp.)

### Krok 3: Aplikuj Migrację do Bazy

```bash
dotnet ef database update \
  --project src/RailVehicleData.Infrastructure \
  --startup-project src/RailVehicleData.WebAPI
```

Lub jeśli SQL Server nie jest dostępny:

```bash
dotnet ef database update --connection "Server=(localdb)\\mssqllocaldb;Database=RailVehicleData;Trusted_Connection=true"
```

### Krok 4: Uruchom Aplikację

```bash
cd src/RailVehicleData.WebAPI
dotnet run
```

Aplikacja automatycznie:
1. Aplikuje pending migrations
2. Seeduje sample data (EU07, SP32, Ty2, EP09 + EMU Class 395, DMU Class 153, Hybrid-220)

## Weryfikacja Bazy Danych

### SQL Server Management Studio (SSMS)

```sql
-- Sprawdź tabele
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'

-- Sprawdź dane w Vehicles
SELECT VehicleId, Manufacturer, Model, ManufacturedYear FROM dbo.Vehicles

-- Sprawdź Traction Systems (z discriminator)
SELECT TractionSystemId, TractionSystemType, VehicleId, InstalledDate FROM dbo.TractionSystems

-- Sprawdź Multiple Units
SELECT MultipleUnitId, Designation, CarCount, Type FROM dbo.MultipleUnits
```

### Via Entity Framework CLI

```bash
# Pokaż informacje o migracji
dotnet ef migrations list \
  --project src/RailVehicleData.Infrastructure

# Pokaż SQL, który będzie wykonany
dotnet ef migrations script \
  --project src/RailVehicleData.Infrastructure \
  --startup-project src/RailVehicleData.WebAPI
```

## Dodatkowe Migracje (Później)

Jeśli zmienisz domenę (np. dodasz pola), wykonaj:

```bash
dotnet ef migrations add [DescriptionOfChange] \
  --project src/RailVehicleData.Infrastructure \
  --startup-project src/RailVehicleData.WebAPI

dotnet ef database update \
  --project src/RailVehicleData.Infrastructure \
  --startup-project src/RailVehicleData.WebAPI
```

## Cofnięcie Migracji (Jeśli Potrzeba)

```bash
# Cofnij ostatnią migrację (dane NIE będą usunięte, jeśli nie ma Down migration)
dotnet ef database update [PreviousMigrationName] \
  --project src/RailVehicleData.Infrastructure \
  --startup-project src/RailVehicleData.WebAPI
```

## Rozwiązywanie Problemów

### Problem: "No EF Core tools installed"

```bash
dotnet tool install --global dotnet-ef
```

### Problem: "DbContext type 'RailVehicleDbContext' not found"

Upewnij się, że:
1. `RailVehicleDbContext` jest public
2. `--startup-project` wskazuje na projekt z `Program.cs`
3. Ścieżki projektów są prawidłowe

### Problem: SQL Server nie dostępny

Zmień connection string na LocalDB:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RailVehicleData;Trusted_Connection=true"
```

### Problem: "The entity type 'Vehicle' requires a primary key"

Tego nie powinno być, ale jeśli się pojawi, sprawdź `RailVehicleDbContext.OnModelCreating()` i upewnij się że `entity.HasKey(v => v.VehicleId)` jest skonfigurowany.

## Seed Data

Seeder automatycznie tworzy:

**Locomotives:**
- EU07 (Electric, 3 kV DC, 3200 kW, 1972)
- SP32 (Diesel, 1200 kW, Euro 0, 1965)
- Ty2 (Steam, narrow gauge, 1952)
- EP09 (Electric modern, 25 kV AC 50 Hz, 6400 kW, 2019)

**Multiple Units:**
- Class 395 (EMU, 8 cars, 300 km/h, 2009)
- Class 153 (DMU, 2 cars, 145 km/h, 1992)
- Hybrid-220 (Hybrid, 4 cars, 250 km/h, 2021)

Aby zmodyfikować seed, edytuj: `src/RailVehicleData.Infrastructure/Seeders/VehicleSeeder.cs`

## Następne Kroki

Po wygenerowaniu migracji możesz:

1. **Tworzyć Controllers** (`src/RailVehicleData.WebAPI/Controllers/`)
2. **Testować Service Layer** (unit tests)
3. **Wdrożyć dodatkowe features** (paging, filtering, validation middleware)

---

**Data**: November 2024
**Architektura**: Clean Architecture + DDD
**ORM**: Entity Framework Core 6.0
**Baza**: SQL Server / LocalDB
