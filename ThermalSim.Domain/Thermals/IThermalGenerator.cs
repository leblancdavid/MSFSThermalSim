using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public interface IThermalGenerator
    {
        Thermal GenerateThermalAroundAircraft(AircraftPositionState position);
    }
}
