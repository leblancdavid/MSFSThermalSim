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

        public ThermalVelocity? GetThermalVelocity(AircraftPositionState position)
        {
            if(!IsInThermal(position))
                return null;

            var liftAmount = 100.0;
            var frameFactor = 0.01;
                
            double xFactor = (-1.0 * position.Bank) / 90.0;
            double yFactor = ((1.0 - Math.Abs(position.Bank) / 90.0) + (1.0 - Math.Abs(position.Pitch) / 90.0)) / 2.0;
            double zFactor = (position.Pitch) / 90.0;

            double xSpeed = position.VelocityBodyX * xFactor;
            double ySpeed = position.VelocityBodyY * yFactor;
            double zSpeed = position.VelocityBodyZ * zFactor;

            if(Math.Abs(xSpeed) < liftAmount)
            {
                if (xSpeed < 0.0)
                    xSpeed -= liftAmount * xFactor * frameFactor;
                else
                    xSpeed += liftAmount * xFactor * frameFactor;
            }
            else
            {
                xSpeed = position.VelocityBodyX;
            }

            if (ySpeed < liftAmount)
            {
                ySpeed += liftAmount * yFactor * frameFactor;
            }
            else
            {
                ySpeed = position.VelocityBodyY;
            }

            if(zSpeed < liftAmount)
            {
                zSpeed += liftAmount * zFactor * frameFactor;
            }
            else
            {
                zSpeed = position.VelocityBodyZ;
            }

            var velocity = new ThermalVelocity()
            {
                VelocityBodyX = xSpeed,
                VelocityBodyY = ySpeed, 
                VelocityBodyZ = zSpeed
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
