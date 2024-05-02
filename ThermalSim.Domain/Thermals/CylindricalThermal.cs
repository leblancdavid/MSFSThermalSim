using System.Diagnostics.CodeAnalysis;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class CylindricalThermal : IThermalModel
    {
        private Random rng = new Random();

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
        public double CoreTurbulencePercent { get; set; }
        public double SinkRate { get; set; }
        public double SinkTurbulencePercent { get; set; }
        public double CoreRadiusPercent { get; set; }
        public double SinkTransitionRadiusPercent { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }

        public double SmoothingFactor { get; set; } = 0.05f;
        public double TimeFactor { get; set; } = 0.03;
        public double LiftModificationFactor { get; set; } = 1.1;
        public double LiftModifier { get; set; } = 0.0;

        public ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange)
        {
            double distance = GetDistanceToThermal(position);

            if(!IsInThermal(position, distance))
                return null;

            if(position.AltitudeAboveGround < MinAltitudeFromGround)
                return null;

            if (stateChange?.AverageVelocity < 50.0) //TODO this number should be based on the plane's stall speed
                return null;

            var lift = CalcBaseLiftValue(position, distance) + UpdateLiftModifier(distance);
            var verticalSpeed = stateChange == null ? position.VerticalSpeed : stateChange.AverageVerticalVelocity;
            if (Math.Abs(verticalSpeed) > Math.Abs(lift))
                return null;

            DebugTrace(position, distance, lift);

            var change = new ThermalAltitudeChange()
            {
                Altitude = position.Altitude + (lift) * TimeFactor,
                VerticalSpeed = (position.VerticalSpeed * (1.0 - SmoothingFactor) + verticalSpeed * SmoothingFactor)
            };

            return change;
        }

        public bool IsInThermal(AircraftPositionState position)
        {
            return IsInThermal(position, GetDistanceToThermal(position));
        }

        private bool IsInThermal(AircraftPositionState position, double calculatedDistance)
        {
            return position.Altitude >= Altitude &&
                position.Altitude <= TopAltitude &&
                position.AltitudeAboveGround > MinAltitudeFromGround &&
                calculatedDistance < TotalRadius;
        }

        public double GetDistanceToThermal(AircraftPositionState position)
        {
            return 20930000 * (Math.Acos(
                Math.Cos(position.Latitude) * Math.Cos(position.Longitude) * Math.Cos(Latitude) * Math.Cos(Longitude) +
                Math.Cos(position.Latitude) * Math.Sin(position.Longitude) * Math.Cos(Latitude) * Math.Sin(Longitude) +
                Math.Sin(position.Latitude) * Math.Sin(Latitude)) / 360.0);
        }

        private double CalcBaseLiftValue(AircraftPositionState position, double distance)
        {
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
                return transitionChange * (atRadius - CoreRadiusPercent) + liftAtTransition;
            }


            double liftAtSinkTransition = SinkRate;
            double sinkChange = ((SinkRate / LiftModificationFactor) - liftAtSinkTransition) / ((1.0 - SinkTransitionRadiusPercent));

            //basic y = mx + b linear equation
            return sinkChange * (atRadius - SinkTransitionRadiusPercent) + liftAtSinkTransition;
        }

        private double UpdateLiftModifier(double distance)
        {
            var m = (2.0 * rng.NextDouble() - 1.0);
            var atRadius = distance / TotalRadius;
            if (atRadius > SinkTransitionRadiusPercent)
            {
                m *= SinkTurbulencePercent * SinkRate;
            }
            else
            {
                m *= CoreTurbulencePercent * CoreLiftRate;
            }

            LiftModifier = LiftModifier * (1.0 - SmoothingFactor) + m * SmoothingFactor;
            return LiftModifier;
        }

        private void DebugTrace(AircraftPositionState position, double distance, double moddedLift)
        {
            string location = "Core";

            var atRadius = distance / TotalRadius;
            if (atRadius < CoreRadiusPercent)
            {
                location = "Core";
            }
            else if (atRadius < SinkTransitionRadiusPercent)
            {
                location = "Transition";
            }
            else
            {
                location = "Sink";
            }


            Console.WriteLine($"{location}: {distance}ft with {moddedLift}ft/s");
        }
    }
}
