using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Turbulence
{
    public class CounterBasedTurbulenceModel : ITurbulenceModel
    {
        private int _minCount;
        private int _maxCount;
        private int _turbulenceCount;
        private int _counter;
        private int _resetCount;
        private double _maxTurbulence;

        private double[] _smoothingKernel;

        private Random _random = new Random();
        
        private TurbulenceEffect? _turbulence = null;

        private double x_scaler = 0.5;
        private double y_scaler = 0.5;
        private double z_scaler = 2.0;


        public CounterBasedTurbulenceModel(int minCount, int maxCount, double maxTurbulence, int turbulenceCount = 20)
        {
            _minCount = minCount;
            _maxCount = maxCount;
            _maxTurbulence = maxTurbulence;
            _turbulenceCount = turbulenceCount;

            double sigma = (double)turbulenceCount / 5.0;
            double f = 1.0 / (Math.Sqrt(2.0 * Math.PI) * sigma);

            _smoothingKernel = new double[turbulenceCount];
            double x = -turbulenceCount / 2.0;
            for (int i = 0; i < turbulenceCount; i++)
            {
                var v = f * Math.Pow(Math.E, -1.0 * x * x / (2.0 * sigma * sigma));
                _smoothingKernel[i] = v;
                x++;
            }

            double max = _smoothingKernel.Max();
            for (int i = 0; i < turbulenceCount; i++)
            {
                _smoothingKernel[i] = 2.0 * (_smoothingKernel[i] / max) - 1.0;
                //_smoothingKernel[i] /= max;
            }

            ResetCount();
        }

        public TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position)
        {
            //If we hit turbulence, this will be not null
            if(_turbulence != null)
            {
                _counter++;
                //If we've hit the number of frames of turbulence, then we reset it
                if(_counter >= _turbulenceCount)
                {
                    _turbulence = null;
                    ResetCount();
                    return _turbulence;
                }

                //otherwise, scale the turbulence affect by the fade factor (gets weaker or stronger with time)
                //_turbulence = new TurbulenceEffect()
                //{
                //    RotationAccelerationBodyX = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * x_scaler,
                //    RotationAccelerationBodyY = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * y_scaler,
                //    RotationAccelerationBodyZ = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * z_scaler,
                //};

                var output = new TurbulenceEffect()
                {
                    RotationAccelerationBodyX = _turbulence.Value.RotationAccelerationBodyX * _smoothingKernel[_counter],
                    RotationAccelerationBodyY = _turbulence.Value.RotationAccelerationBodyY * _smoothingKernel[_counter],
                    RotationAccelerationBodyZ = _turbulence.Value.RotationAccelerationBodyZ * _smoothingKernel[_counter],
                };

                return output;
            }

            //This counter makes it so that turbulence is spread apart somewhat randomly
            if(_counter < _resetCount)
            {
                _counter++;
                return null;
            }

            //If we get here, we basically start a new turbulence
            ResetCount();

            //For now let's make it simple
            _turbulence = new TurbulenceEffect()
            {
                RotationAccelerationBodyX = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * x_scaler,
                RotationAccelerationBodyY = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * y_scaler,
                RotationAccelerationBodyZ = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * z_scaler,
            };

            return _turbulence;
        }

        private void ResetCount()
        {
            _resetCount = _random.Next(_minCount, _maxCount);
            _counter = 0;
        }
    }
}
