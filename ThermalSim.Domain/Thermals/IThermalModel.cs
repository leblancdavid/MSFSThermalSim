using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public interface IThermalModel
    {
        public uint ObjectId { get; set; }
        ThermalProperties Properties { get; set; }

        ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange);
        bool IsInThermal(AircraftPositionState position);
        double GetDistanceToThermal(AircraftPositionState position);
    }
}
