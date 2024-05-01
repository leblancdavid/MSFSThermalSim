﻿using ThermalSim.Domain.Position;

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

        public ThermalVelocity? GetThermalVelocity(AircraftPositionState position)
        {
            if(!IsInThermal(position))
                return null;

            //TODO actually do the work but for now

            var liftAmount = CoreRate;

            var velocity = new ThermalVelocity()
            {
                VelocityBodyY = 0.0, // (position.VelocityBodyY + liftAmount) / 2.0,
                VelocityBodyZ = (position.VelocityBodyZ + liftAmount / 2.0) / 2.0,
                RotationAccelerationBodyX = 0.0 //position.RotationAccelerationBodyX + liftAmount * Math.Max(0, Math.Min(0.34, 0.17 - (position.Pitch + position.RotationVelocityBodyX))),
            };

            return velocity;
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
