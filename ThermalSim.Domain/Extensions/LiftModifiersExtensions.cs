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

    }
}
