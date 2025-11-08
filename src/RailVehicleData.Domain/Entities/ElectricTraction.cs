namespace RailVehicleData.Domain.Entities;

/// <summary>
/// Represents an electric traction system (electric locomotive/EMU).
/// Supports AC (25 kV 50 Hz typical in Europe) and DC (1.5 kV or 3 kV) systems.
/// </summary>
public sealed class ElectricTraction : TractionSystem
{
    /// <summary>
    /// Voltage of the overhead line system (e.g., "25 kV AC", "1.5 kV DC", "3 kV DC").
    /// </summary>
    public string VoltageSystem { get; }

    /// <summary>
    /// Frequency of AC systems (50 Hz in Europe, 60 Hz in USA).
    /// Null for DC systems.
    /// </summary>
    public Frequency? ACFrequency { get; }

    /// <summary>
    /// Indicates if this system has regenerative braking capability.
    /// </summary>
    public bool HasRegenerativeBraking { get; }

    /// <summary>
    /// Type of pantograph equipment.
    /// </summary>
    public string PantographType { get; }

    public ElectricTraction(
        Guid vehicleId,
        DateTime installedDate,
        Power maxPower,
        string voltageSystem,
        bool hasRegenerativeBraking,
        string pantographType,
        Frequency? acFrequency = null) : base(vehicleId, installedDate, maxPower)
    {
        if (string.IsNullOrWhiteSpace(voltageSystem))
            throw new DomainException("Voltage system cannot be empty.");

        if (string.IsNullOrWhiteSpace(pantographType))
            throw new DomainException("Pantograph type cannot be empty.");

        VoltageSystem = voltageSystem;
        HasRegenerativeBraking = hasRegenerativeBraking;
        PantographType = pantographType;
        ACFrequency = acFrequency;
    }
}
