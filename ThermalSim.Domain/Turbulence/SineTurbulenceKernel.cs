namespace ThermalSim.Domain.Turbulence
{
    public class SineTurbulenceKernel : ITurbulenceKernel
    {
        private readonly double piWaves;

        public SineTurbulenceKernel(double piWaves = 3.0) 
        {
            this.piWaves = piWaves;
        }
        public double[] GetTurbulenceKernel(int duration)
        {
            double incr = (Math.PI * piWaves) / (double)duration;

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
