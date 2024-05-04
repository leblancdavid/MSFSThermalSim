using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Turbulence
{
    public interface ITurbulenceModel
    {
        TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position);
    }
}
