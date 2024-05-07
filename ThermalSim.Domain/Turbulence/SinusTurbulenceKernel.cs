namespace ThermalSim.Domain.Turbulence
{
    public class SinusTurbulenceKernel : ITurbulenceKernel
    {
        public double[] GetTurbulenceKernel(int duration)
        {
            double incr = (Math.PI * 3) / (double)duration;

            var kernel = new double[duration];
            double sigma = 1.0;
            double sigmaIncr = 1.0 / (double)duration;
            double x = 0;
            for (int i = 0; i < duration; i++)
            {
                kernel[i] = sigma* Math.Sin(x);
                x += incr;
                sigma -= sigmaIncr;
            }

            return kernel;
        }
    }
}
