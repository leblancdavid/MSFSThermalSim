using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public static class LiftModifiersExtention
    {
        public static double ApplyStallModifier(this double lift, AircraftPositionState position)
        {
            if (position.StallWarning > 0)
            {
                if (lift > 0.0)
                {
                    return 0.5 * lift;
                }

                return 1.1 * lift;
            }

            return lift;

        }

        public static double ApplySpoilerModifier(this double lift, AircraftPositionState position)
        {
            if (lift > 0.0)
            {
                return (-0.5 * (position.SpoilerHandlePosition) + 1.0) * lift;
            }

            return lift;
        }

        public static double ApplyWeightModifier(this double lift, AircraftPositionState position)
        {
            var maxWeightDiff = position.MaxGrossWeight - position.EmptyWeight;
            var currentWeightDiff = position.TotalWeight - position.EmptyWeight;
            var weightRatio = currentWeightDiff / maxWeightDiff;

            if (lift > 0.0)
            {
                return ((0.5 - weightRatio) / 2.0 + 1.0) * lift;
            }

            return ((weightRatio - 0.5) / 2.0 + 1.0) * lift;
        }

    }
}
