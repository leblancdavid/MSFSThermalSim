using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class CylindricalThermal : IThermalModel
    {
        public uint ObjectId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public float TopAltitude => Altitude + Height;
        public float Radius { get; set; }
        public float Height { get; set; }
        public float CoreRate { get; set; }
        public float CoreTurbulence { get; set; }
        public float SinkRate { get; set; }
        public float SinkTurbulence { get; set; }
        public float CoreSize { get; set; }
        public float CoreTransitionSize { get; set; }
        public float SinkLayerSize { get; set; }
        public float SinkTransitionSize { get; set; }
        public float WindSpeed { get; set; }
        public float WindDirection { get; set; }

        public ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange)
        {
            if(!IsInThermal(position))
                return null;

            var lift = 20.0f;
            var verticalSpeed = stateChange == null ? position.VerticalSpeed : stateChange.AverageVerticalVelocity;
            if(verticalSpeed > lift)
                return null;

            var change = new ThermalAltitudeChange()
            {
                Altitude = position.Altitude + (lift + verticalSpeed) * 0.01,
                VerticalSpeed = (position.VerticalSpeed * 0.95 + verticalSpeed * 0.05)
            };

            return change;
        }

        public bool IsInThermal(AircraftPositionState position)
        {
            return GetDistanceToThermal(position) < Radius && 
                position.Altitude >= Altitude && 
                position.Altitude <= TopAltitude;
        }

        public float GetDistanceToThermal(AircraftPositionState position)
        {
            return 0.0f;
            //For now, we'll assume a straight up cylindrical model
        }
    }
}
