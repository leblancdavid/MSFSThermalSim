using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Extensions
{
    public static class LiftModifiersExtensions
    {
        public static double ApplyStallModifier(this ref double lift, AircraftPositionState position)
        {
            if (position.StallWarning > 0)
            {
                if (lift > 0.0)
                {
                    lift *= 0.5;
                }
                else
                {
                    lift *= 1.1;
                }
            }

            return lift;

        }

        public static double ApplySpoilerModifier(this ref double lift, AircraftPositionState position)
        {
            if (lift > 0.0)
            {
                lift = (-0.5 * (position.SpoilerHandlePosition / 100.0) + 1.0) * lift;
            }

            return lift;
        }

        public static double ApplyWeightModifier(this ref double lift, AircraftPositionState position)
        {
            var maxWeightDiff = position.MaxGrossWeight - position.EmptyWeight;
            var currentWeightDiff = position.TotalWeight - position.EmptyWeight;
            var weightRatio = currentWeightDiff / maxWeightDiff;

            if (lift > 0.0)
            {
                lift *= (0.5 - weightRatio) / 2.0 + 1.0;
                return lift;
            }

            lift *= (weightRatio - 0.5) / 2.0 + 1.0;
            return lift;
        }

        public static double ApplyAboveGroundAltitudeModifier(this ref double lift, AircraftPositionState position, double maxAboveGroundAltitude)
        {
            if (position.AltitudeAboveGround < maxAboveGroundAltitude)
            {
                lift *= position.AltitudeAboveGround / maxAboveGroundAltitude;
            }

            return lift;
        }

        public static double ApplyThermalAltitudeModifier(this ref double lift, double percentHeightInThermal, double weakeningFactor = 0.9, double liftHeightThreshold = 0.95)
        {
            double topLift = lift * weakeningFactor;

            lift -= percentHeightInThermal * (lift - topLift);

            //At the top of the thermal, the amount of lift drops off
            if(percentHeightInThermal > liftHeightThreshold)
            {
                lift *= (1.0 - percentHeightInThermal) / (1.0 - liftHeightThreshold);
            }

            return lift;
        }

    }
}
