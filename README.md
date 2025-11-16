# RailVehicleData API

API do zarządzania danymi technicznymi pojazdów szynowych, zbudowane w architekturze Clean Architecture z wykorzystaniem .NET 6.0.

## 📋 Spis treści

- [Opis projektu](#opis-projektu)
- [Architektura](#architektura)
- [Technologie](#technologie)
- [Struktura projektu](#struktura-projektu)
- [Wymagania](#wymagania)
- [Instalacja](#instalacja)
- [Uruchomienie](#uruchomienie)
- [API Endpoints](#api-endpoints)
- [Funkcjonalności](#funkcjonalności)

## 🎯 Opis projektu

RailVehicleData to aplikacja webowa typu REST API, która umożliwia zarządzanie bazą danych technicznych różnych typów pojazdów szynowych, ze szczególnym uwzględnieniem lokomotyw elektrycznych. System został zaprojektowany zgodnie z wzorcem Clean Architecture, co zapewnia wysoką testowalność, łatwość utrzymania i niezależność od szczegółów implementacyjnych.

## 🏗️ Architektura

Projekt został zbudowany zgodnie z zasadami **Clean Architecture**, dzieląc system na cztery warstwy:

### 1. Domain Layer (RailVehicleData.Domain)
- **Odpowiedzialność**: Encje biznesowe i logika domenowa
- **Zależności**: Brak (warstwa najbardziej wewnętrzna)
- **Zawiera**:
  - `Entities/Vehicle.cs` - Bazowa encja pojazdu
  - `Entities/ElectricVehicle.cs` - Encja pojazdu elektrycznego
  - `Interfaces/IVehicleRepository.cs` - Kontrakt repozytorium

### 2. Application Layer (RailVehicleData.Application)
- **Odpowiedzialność**: Logika aplikacji, use cases, DTOs
- **Zależności**: Domain Layer
- **Zawiera**:
  - `Dto/` - Obiekty transferu danych (NewVehicleDto, VehicleDto)
  - `Interfaces/IVehicleService.cs` - Kontrakt serwisu
  - `Services/VehicleService.cs` - Implementacja logiki biznesowej
  - `Mappings/AutoMapperConfig.cs` - Konfiguracja mapowań

### 3. Infrastructure Layer (RailVehicleData.Infrastructure)
- **Odpowiedzialność**: Dostęp do danych, zewnętrzne serwisy
- **Zależności**: Domain Layer
- **Zawiera**:
  - `Data/RailVehicleDbContext.cs` - Kontekst Entity Framework
  - `Repositories/VehicleRepository.cs` - Implementacja repozytorium

### 4. Presentation Layer (RailVehicleData.WebAPI)
- **Odpowiedzialność**: API, kontrolery, middleware
- **Zależności**: Application Layer, Infrastructure Layer
- **Zawiera**:
  - `Controllers/RailVehicleController.cs` - REST API endpoints
  - `Middleware/GlobalExceptionHandlerMiddleware.cs` - Obsługa błędów
  - `Program.cs` - Konfiguracja aplikacji

## 🛠️ Technologie

- **.NET 6.0** - Framework aplikacji
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 6.0.16** - ORM
- **SQL Server** - Baza danych
- **AutoMapper 12.0.1** - Mapowanie obiektów
- **Swagger/OpenAPI** - Dokumentacja API

## 📁 Struktura projektu

```
RailVehicleData/
├── RailVehicleData.Domain/
│   ├── Entities/
│   │   ├── Vehicle.cs
│   │   └── ElectricVehicle.cs
│   ├── Interfaces/
│   │   └── IVehicleRepository.cs
│   └── RailVehicleData.Domain.csproj
│
├── RailVehicleData.Application/
│   ├── Dto/
│   │   ├── NewVehicleDto.cs
│   │   └── VehicleDto.cs
│   ├── Interfaces/
│   │   └── IVehicleService.cs
│   ├── Services/
│   │   └── VehicleService.cs
│   ├── Mappings/
│   │   └── AutoMapperConfig.cs
│   └── RailVehicleData.Application.csproj
│
├── RailVehicleData.Infrastructure/
│   ├── Data/
│   │   └── RailVehicleDbContext.cs
│   ├── Repositories/
│   │   └── VehicleRepository.cs
│   └── RailVehicleData.Infrastructure.csproj
│
├── RailVehicleData.WebAPI/
│   ├── Controllers/
│   │   └── RailVehicleController.cs
│   ├── Middleware/
│   │   └── GlobalExceptionHandlerMiddleware.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── RailVehicleData.WebAPI.csproj
│
└── RailVehicleData.sln
```

## ✅ Wymagania

- .NET 6.0 SDK
- SQL Server (LocalDB lub pełna wersja)
- Visual Studio 2022 / VS Code / Rider (opcjonalnie)

## 🚀 Instalacja

### 1. Sklonuj repozytorium

```bash
git clone <repository-url>
cd RailVehicleData
```

### 2. Przywróć pakiety NuGet

```bash
dotnet restore
```

### 3. Skonfiguruj connection string

Edytuj plik `appsettings.json` w projekcie WebAPI:

```json
{
  "ConnectionStrings": {
    "RailVehicleCS": "Server=(localdb)\\mssqllocaldb;Database=RailVehicleDb;Trusted_Connection=True;"
  }
}
```

### 4. Utwórz bazę danych

Baza danych jest tworzona automatycznie przy pierwszym uruchomieniu aplikacji dzięki `EnsureCreated()`.

Alternatywnie możesz użyć migracji Entity Framework:

```bash
cd RailVehicleData.WebAPI
dotnet ef migrations add InitialCreate --project ../RailVehicleData.Infrastructure
dotnet ef database update
```

## 🎮 Uruchomienie

### Uruchomienie z linii poleceń

```bash
cd RailVehicleData
dotnet run --project RailVehicleData.WebAPI
```

### Uruchomienie w Visual Studio

1. Otwórz plik `RailVehicleData.sln`
2. Ustaw `RailVehicleData.WebAPI` jako projekt startowy
3. Naciśnij F5 lub kliknij "Start"

### Dostęp do aplikacji

- **API**: https://localhost:7056
- **Swagger UI**: https://localhost:7056 (root URL)
- **HTTP**: http://localhost:5021

## 📡 API Endpoints

### GET /api/RailVehicle
Pobiera wszystkie pojazdy szynowe

**Response**: `200 OK`
```json
[
  {
    "vehicleId": 1,
    "model": "EU07",
    "manufacturer": "Pafawag",
    "productionYear": 1965,
    "maxSpeed": 125,
    "weight": 84.00,
    "powerType": "Elektryczny",
    "voltage": "3000V",
    "currentType": "DC"
  }
]
```

### GET /api/RailVehicle/{id}
Pobiera pojazd o określonym ID

**Parameters**: `id` (int) - Identyfikator pojazdu

**Response**: `200 OK` | `404 Not Found` | `400 Bad Request`

### POST /api/RailVehicle
Tworzy nowy pojazd standardowy

**Request Body**:
```json
{
  "model": "SM42",
  "manufacturer": "Fablok",
  "productionYear": 1975,
  "maxSpeed": 90,
  "weight": 82.00,
  "powerType": "Spalinowy"
}
```

**Response**: `201 Created`

### POST /api/RailVehicle/ElectricVehicles
Tworzy nowy pojazd elektryczny

**Request Body**:
```json
{
  "model": "EU07",
  "manufacturer": "Pafawag",
  "productionYear": 1965,
  "maxSpeed": 125,
  "weight": 84.00,
  "powerType": "Elektryczny",
  "voltage": "3000V",
  "currentType": "DC",
  "enginePower": 2000,
  "numberOfMotors": 4,
  "motorType": "Prądu stałego",
  "wheelArrangement": "Bo'Bo'",
  "isMultiSystem": false,
  "maxTractiveEffort": 240
}
```

**Response**: `201 Created`

### PUT /api/RailVehicle/{id}
Aktualizuje istniejący pojazd

**Parameters**: `id` (int) - Identyfikator pojazdu

**Request Body**: Dane do aktualizacji (format jak w POST)

**Response**: `204 No Content` | `404 Not Found` | `400 Bad Request`

### DELETE /api/RailVehicle/{id}
Usuwa pojazd o określonym ID

**Parameters**: `id` (int) - Identyfikator pojazdu

**Response**: `204 No Content` | `404 Not Found` | `400 Bad Request`

## ✨ Funkcjonalności

### Zaimplementowane

- ✅ CRUD operations dla pojazdów szynowych
- ✅ Wsparcie dla dwóch typów pojazdów (standardowy, elektryczny)
- ✅ Walidacja danych wejściowych (Data Annotations)
- ✅ Globalna obsługa błędów (Middleware)
- ✅ Logging (ILogger)
- ✅ Dokumentacja API (Swagger/OpenAPI)
- ✅ Mapowanie DTO ↔ Entity (AutoMapper)
- ✅ Seed data (przykładowe dane w bazie)
- ✅ Clean Architecture pattern
- ✅ Dependency Injection
- ✅ Async/await operations
- ✅ CORS support

### Dane początkowe

Po uruchomieniu aplikacji baza danych jest automatycznie wypełniana przykładowymi danymi:
- **SM42** - Lokomotywa spalinowa Fablok (1975)
- **SP32** - Lokomotywa spalinowa Fablok (1969)
- **EU07** - Elektrowóz Pafawag (1965)
- **EP09** - Elektrowóz Pafawag (1986)

## 🔧 Konfiguracja

### appsettings.json

```json
{
  "ConnectionStrings": {
    "RailVehicleCS": "Server=(localdb)\\mssqllocaldb;Database=RailVehicleDb;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 🧪 Testowanie

Możesz przetestować API używając:

1. **Swagger UI** - https://localhost:7056
2. **Postman** - Zaimportuj kolekcję z Swagger JSON
3. **curl** - Przykład:

```bash
curl -X GET "https://localhost:7056/api/RailVehicle" -H "accept: application/json"
```

## 📝 Uwagi

- Projekt używa **Table Per Hierarchy (TPH)** dla dziedziczenia encji (Vehicle → ElectricVehicle)
- Baza danych jest automatycznie tworzona przy pierwszym uruchomieniu
- XML documentation jest generowana automatycznie dla Swagger
- Wszystkie operacje są asynchroniczne dla lepszej wydajności

## 🤝 Współpraca

1. Fork projektu
2. Utwórz branch feature (`git checkout -b feature/AmazingFeature`)
3. Commit zmian (`git commit -m 'Add some AmazingFeature'`)
4. Push do brancha (`git push origin feature/AmazingFeature`)
5. Otwórz Pull Request

## 📄 Licencja

Projekt stworzony do celów edukacyjnych.

---

**Autor**: RailVehicleData Team
**Wersja**: 1.0.0
**Data**: 2024
