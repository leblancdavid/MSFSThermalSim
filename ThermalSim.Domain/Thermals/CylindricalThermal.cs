﻿using System.Reflection.Metadata.Ecma335;
using ThermalSim.Domain.Extensions;
using ThermalSim.Domain.Position;
using ThermalSim.Domain.Turbulence;

namespace ThermalSim.Domain.Thermals
{
    public class CylindricalThermal : IThermalModel
    {
        private Random rng = new Random();

        public uint ObjectId { get; set; }
        public ThermalProperties Properties { get; set; } = new ThermalProperties();

        public double SmoothingFactor { get; set; } = 0.05f;
        public double TimeFactor { get; set; } = Constants.DEFAULT_TIME_FACTOR;
        public double LiftModifier { get; set; } = 0.0;
        public ITurbulenceModel TurbulenceModel { get; set; } = new CounterBasedTurbulenceModel(new TurbulenceProperties());

        public ThermalAltitudeChange? GetThermalAltitudeChange(AircraftPositionState position, AircraftStateChangeInfo? stateChange)
        {
            double distance = GetDistanceToThermal(position);

            if(!IsInThermal(position, distance))
                return null;

            if(position.AltitudeAboveGround < Properties.MinAltitudeFromGround)
                return null;

            var lift = CalcBaseLiftValue(distance, stateChange) + UpdateLiftModifier(distance);

            var verticalSpeed = stateChange == null ? position.VerticalSpeed : stateChange.AverageVerticalVelocity;
            if (Math.Abs(verticalSpeed) > Math.Abs(lift))
                return null;

            lift.ApplyAboveGroundAltitudeModifier(position, Properties.MaxAltitudeFromGround);
            lift.ApplyThermalAltitudeModifier((position.Altitude - Properties.Altitude) / Properties.Height);
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

        public void ApplyWindDrift(double direction, double windSpeedFps)
        {
            this.CalcApplyWindDrift(direction, windSpeedFps);
        }



        private double CalcBaseLiftValue(double distance, AircraftStateChangeInfo? stateChange)
        {
            var atRadius = distance / Properties.TotalRadius;
            if (atRadius > 1.0)
            {
                if(stateChange != null)
                    stateChange.ThermalState = ThermalPositionState.NotInThermal;

                return 0.0;
            }

            if(atRadius < Properties.CoreRadiusPercent)
            {
                if (stateChange != null)
                    stateChange.ThermalState = ThermalPositionState.InThermalCore;

                double liftAtCenter = (Properties.LiftShapeFactor * 0.5 + 1.0) * Properties.CoreLiftRate;
                double liftChange = (Properties.CoreLiftRate - liftAtCenter) / Properties.CoreRadiusPercent;
                //basic y = mx + b linear equation
                return liftChange * atRadius + liftAtCenter; 
            }
            else if(atRadius < Properties.SinkTransitionRadiusPercent)
            {
                if (stateChange != null)
                    stateChange.ThermalState = ThermalPositionState.InThermalTransition;

                double liftAtTransition = Properties.CoreLiftRate;
                double transitionChange = (Properties.SinkRate - liftAtTransition) / (Properties.SinkTransitionRadiusPercent - Properties.CoreRadiusPercent);
                //basic y = mx + b linear equation
                return transitionChange * (atRadius - Properties.CoreRadiusPercent) + liftAtTransition;
            }

            if (stateChange != null)
                stateChange.ThermalState = ThermalPositionState.InThermalSink;

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
