namespace ThermalSim.Domain.Turbulence
{
    public interface ITurbulenceKernel
    {
        double[] GetTurbulenceKernel(int duration);

    }
}
