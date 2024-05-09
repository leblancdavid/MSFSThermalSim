using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public interface ITurbulenceModel
    {
        TurbulenceProperties Properties { get; set; }
        TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position, IThermalModel thermal);
    }
}
