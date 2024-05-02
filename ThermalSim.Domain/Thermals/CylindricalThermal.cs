using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class CylindricalThermal : IThermalModel
    {
        public uint ObjectId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double MinAltitudeFromGround { get; set; } = 100.0f;
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

        public double SmoothingFactor { get; set; } = 0.05f;
        public double TimeFactor { get; set; } = 0.0167;
        public double LiftModificationFactor { get; set; } = 1.1;

        public ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange)
        {
            if(!IsInThermal(position))
                return null;

            if(position.AltitudeAboveGround < MinAltitudeFromGround)
                return null;

            if (stateChange?.AverageVelocity < 50.0) //TODO this number should be based on the plane's stall speed
                return null;

            var lift = CalcBaseLiftValue(position);
            var verticalSpeed = stateChange == null ? position.VerticalSpeed : stateChange.AverageVerticalVelocity;
            if (verticalSpeed > lift)
                return null;

            var change = new ThermalAltitudeChange()
            {
                Altitude = position.Altitude + (lift) * TimeFactor,
                VerticalSpeed = (position.VerticalSpeed * (1.0 - SmoothingFactor) + verticalSpeed * SmoothingFactor)
            };

            return change;
        }

        public bool IsInThermal(AircraftPositionState position)
        {
            return position.Altitude >= Altitude && 
                position.Altitude <= TopAltitude && 
                position.AltitudeAboveGround > MinAltitudeFromGround && 
                GetDistanceToThermal(position) < TotalRadius;
        }

        public double GetDistanceToThermal(AircraftPositionState position)
        {
            return Math.Sqrt(Math.Pow(position.Latitude - Latitude, 2.0) +
                Math.Pow(position.Longitude - Longitude, 2.0));
        }

        private double CalcBaseLiftValue(AircraftPositionState position)
        {
            var distance = GetDistanceToThermal(position);
            var atRadius = distance / TotalRadius;
            if (atRadius > 1.0)
                return 0.0;

            if(atRadius < CoreRadiusPercent)
            {
                double liftAtCenter = LiftModificationFactor * CoreLiftRate;
                double liftChange = (CoreLiftRate - liftAtCenter) / CoreRadiusPercent;
                //basic y = mx + b linear equation
                return liftChange * atRadius + liftAtCenter; 
            }
            else if(atRadius < SinkTransitionRadiusPercent) 
            {
                double liftAtTransition = CoreLiftRate;
                double transitionChange = (SinkRate - liftAtTransition) / (SinkTransitionRadiusPercent - CoreRadiusPercent);
                //basic y = mx + b linear equation
                return transitionChange * atRadius + liftAtTransition;
            }


            double liftAtSinkTransition = SinkRate;
            double sinkChange = ((SinkRate / LiftModificationFactor) - liftAtSinkTransition) / (1.0 - SinkTransitionRadiusPercent);

            //basic y = mx + b linear equation
            return sinkChange * atRadius + liftAtSinkTransition;
        }
    }
}
