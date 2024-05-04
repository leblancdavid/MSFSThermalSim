using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public interface IThermalModel
    {
        public uint ObjectId { get; set; }
        ThermalProperties Properties { get; set; }

        ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange);
        bool IsInThermal(AircraftPositionState position); 
        bool IsInThermal(double latitude, double longitude, double altitude);
        double GetDistanceToThermal(AircraftPositionState position);
        double GetDistanceToThermal(double latitude, double longitude);
    }
}
