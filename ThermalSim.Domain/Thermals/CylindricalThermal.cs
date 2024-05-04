using System.Reflection.Metadata.Ecma335;
using ThermalSim.Domain.Extensions;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class CylindricalThermal : IThermalModel
    {
        private Random rng = new Random();

        public uint ObjectId { get; set; }
        public ThermalProperties Properties { get; set; } = new ThermalProperties();

        public double SmoothingFactor { get; set; } = 0.05f;
        public double TimeFactor { get; set; } = 0.02;
        public double LiftModifier { get; set; } = 0.0;

        public ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange)
        {
            double distance = GetDistanceToThermal(position);

            if(!IsInThermal(position, distance))
                return null;

            if(position.AltitudeAboveGround < Properties.MinAltitudeFromGround)
                return null;

            if (stateChange?.AverageVelocity < 50.0) //TODO this number should be based on the plane's stall speed
                return null;

            var lift = CalcBaseLiftValue(distance) + UpdateLiftModifier(distance);

            var verticalSpeed = stateChange == null ? position.VerticalSpeed : stateChange.AverageVerticalVelocity;
            if (Math.Abs(verticalSpeed) > Math.Abs(lift))
                return null;

            lift.ApplyStallModifier(position);
            lift.ApplySpoilerModifier(position);
            lift.ApplyWeightModifier(position);

            var verticalSpeedIndicator = (position.VerticalSpeed * (1.0 - SmoothingFactor) + verticalSpeed * SmoothingFactor);
            var change = new ThermalAltitudeChange()
            {
                Altitude = position.Altitude + (lift) * TimeFactor,
                VerticalSpeed = verticalSpeedIndicator,
                PanelVerticalSpeed = verticalSpeedIndicator
            };

            //DebugTrace(position, distance, lift);

            return change;
        }

        public bool IsInThermal(AircraftPositionState position)
        {
            return IsInThermal(position, GetDistanceToThermal(position));
        }

        private bool IsInThermal(AircraftPositionState position, double calculatedDistance)
        {
            return position.Altitude >= Properties.Altitude &&
                position.Altitude <= Properties.TopAltitude &&
                position.AltitudeAboveGround > Properties.MinAltitudeFromGround &&
                calculatedDistance < Properties.TotalRadius;
        }

        public bool IsInThermal(double latitude, double longitude, double altitude)
        {
            var distance = GetDistanceToThermal(latitude, longitude);
            return altitude >= Properties.Altitude &&
                altitude <= Properties.TopAltitude &&
                distance < Properties.TotalRadius;
        }

        public double GetDistanceToThermal(AircraftPositionState position)
        {
            return position.CalcDistance(this);
        }

        public double GetDistanceToThermal(double latitude, double longitude)
        {
            return this.CalcDistance(latitude, longitude);
        }

        private double CalcBaseLiftValue(double distance)
        {
            var atRadius = distance / Properties.TotalRadius;
            if (atRadius > 1.0)
                return 0.0;

            if(atRadius < Properties.CoreRadiusPercent)
            {
                double liftAtCenter = (Properties.LiftShapeFactor * 0.5 + 1.0) * Properties.CoreLiftRate;
                double liftChange = (Properties.CoreLiftRate - liftAtCenter) / Properties.CoreRadiusPercent;
                //basic y = mx + b linear equation
                return liftChange * atRadius + liftAtCenter; 
            }
            else if(atRadius < Properties.SinkTransitionRadiusPercent) 
            {
                double liftAtTransition = Properties.CoreLiftRate;
                double transitionChange = (Properties.SinkRate - liftAtTransition) / (Properties.SinkTransitionRadiusPercent - Properties.CoreRadiusPercent);
                //basic y = mx + b linear equation
                return transitionChange * (atRadius - Properties.CoreRadiusPercent) + liftAtTransition;
            }


            double liftAtSinkTransition = Properties.SinkRate;
            double sinkChange = ((Properties.SinkRate / (Properties.LiftShapeFactor * 0.5 + 1.0)) - liftAtSinkTransition) / ((1.0 - Properties.SinkTransitionRadiusPercent));

            //basic y = mx + b linear equation
            return sinkChange * (atRadius - Properties.SinkTransitionRadiusPercent) + liftAtSinkTransition;
        }

        private double UpdateLiftModifier(double distance)
        {
            var m = (2.0 * rng.NextDouble() - 1.0);
            var atRadius = distance / Properties.TotalRadius;
            if (atRadius > Properties.SinkTransitionRadiusPercent)
            {
                m *= Properties.SinkTurbulencePercent * Properties.SinkRate;
            }
            else
            {
                m *= Properties.CoreTurbulencePercent * Properties.CoreLiftRate;
            }

            LiftModifier = LiftModifier * (1.0 - SmoothingFactor) + m * SmoothingFactor;
            return LiftModifier;
        }

        
        private void DebugTrace(AircraftPositionState position, double distance, double modifiedLift)
        {
            string location = "Core";

            var atRadius = distance / Properties.TotalRadius;
            if (atRadius < Properties.CoreRadiusPercent)
            {
                location = "Core";
            }
            else if (atRadius < Properties.SinkTransitionRadiusPercent)
            {
                location = "Transition";
            }
            else
            {
                location = "Sink";
            }


            Console.WriteLine($"{location}: {distance}ft with {modifiedLift}ft/s");
        }

        
    }
}
