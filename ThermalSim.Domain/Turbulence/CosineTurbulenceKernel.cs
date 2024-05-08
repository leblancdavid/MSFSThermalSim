namespace ThermalSim.Domain.Turbulence
{
    public class CosineTurbulenceKernel : ITurbulenceKernel
    {
        private readonly double period;
        private readonly double scaler;
        private readonly double translation;

        public CosineTurbulenceKernel(double period = 3.0, double scaler = 1.0, double translation = 0.0)
        {
            this.period = period;
            this.scaler = scaler;
            this.translation = translation;
        }

        public double[] GetTurbulenceKernel(int duration)
        {
            double incr = (Math.PI * period) / (double)duration;

            var kernel = new double[duration];
            double sigma = scaler;
            double sigmaIncr = scaler / (double)duration;
            double x = 0;
            for (int i = 0; i < duration; i++)
            {
                kernel[i] = sigma* Math.Cos(x) + translation;
                x += incr;
                sigma -= sigmaIncr;
            }

            return kernel;
        }
    }
}
