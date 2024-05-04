using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public class CounterBasedTurbulenceModel : ITurbulenceModel
    {
        private int _counter;
        private int _resetCount;
        private int _duration;
        private double _maxTurbulence;

        private double[] _smoothingKernel = null;

        private Random _random = new Random();
        
        private TurbulenceEffect? _turbulence = null;

        public TurbulenceProperties Properties { get; set; }

        public CounterBasedTurbulenceModel(TurbulenceProperties properties)
        {
            Properties = properties;
        }

        private void UpdatesSmoothingKernel(int duration)
        {
            double incr = (Math.PI * 3) / (double) duration;

            _smoothingKernel = new double[duration];
            double x = 0;
            for (int i = 0; i < duration; i++)
            {
                _smoothingKernel[i] = Math.Sin(x);
                x += incr;
            }
        }

        public TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position)
        {
            //If we hit turbulence, this will be not null
            if(_turbulence != null)
            {
                _counter++;
                //If we've hit the number of frames of turbulence, then we reset it
                if(_counter >= _duration)
                {
                    _turbulence = null;
                    ResetCount();
                    return _turbulence;
                }

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

            UpdatesSmoothingKernel(_duration);

            //For now let's make it simple
            _turbulence = new TurbulenceEffect()
            {
                RotationAccelerationBodyX = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * Properties.x_Scaler,
                RotationAccelerationBodyY = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * Properties.y_Scaler,
                RotationAccelerationBodyZ = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * Properties.z_Scaler,
            };

            return _turbulence;
        }

        private void ResetCount()
        {
            _resetCount = Properties.FramesBetweenTurbulence.GetRandomValue(_random);
            _duration = Properties.TurbulenceDuration.GetRandomValue(_random);
            _maxTurbulence = Properties.TurbulenceStrength.GetRandomValue(_random);
            _counter = 0;
        }
    }
}
