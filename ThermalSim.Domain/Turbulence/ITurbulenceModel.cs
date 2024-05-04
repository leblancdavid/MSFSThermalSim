using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public interface ITurbulenceModel
    {
        TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position, IThermalModel thermal);
    }
}
