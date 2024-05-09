namespace ThermalSim.Domain.Turbulence
{
    public class CosineTurbulenceKernel : ITurbulenceKernel
    {
        private readonly double period;
        private readonly double shift;
        private readonly double scaler;
        private readonly double translation;

        public CosineTurbulenceKernel(double period = 3.0, double shift = 0.0, double scaler = 1.0, double translation = 0.0)
        {
            this.period = period;
            this.shift = shift;
            this.scaler = scaler;
            this.translation = translation;
        }

        public double[] GetTurbulenceKernel(int duration)
        {
            double incr = (Math.PI * period) / (double)duration;

            var kernel = new double[duration];
            double sigma = 1.0;
            double sigmaIncr = scaler / (double)duration;
            double x = shift;
            for (int i = 0; i < duration; i++)
            {
                kernel[i] = sigma * (scaler * Math.Cos(x) + translation);
                x += incr;
                sigma -= sigmaIncr;
            }

            return kernel;
        }
    }
}
