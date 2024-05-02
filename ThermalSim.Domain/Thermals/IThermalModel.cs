using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public interface IThermalModel
    {
        public uint ObjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double TopAltitude => Altitude + Height;
        public double TotalRadius { get; set; }
        public double Height { get; set; }
        public double CoreLiftRate { get; set; }
        public double CoreTurbulence { get; set; }
        public double SinkRate { get; set; }
        public double SinkTurbulence { get; set; }
        public double CoreRadiusPercent { get; set; }
        public double SinkTransitionRadiusPercent { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }

        ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange);
        bool IsInThermal(AircraftPositionState position);
        double GetDistanceToThermal(AircraftPositionState position);
    }
}
