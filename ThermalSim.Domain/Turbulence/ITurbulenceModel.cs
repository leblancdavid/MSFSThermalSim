using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Turbulence
{
    public interface ITurbulenceModel
    {
        TurbulenceProperties Properties { get; set; }
        TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position);
    }
}
