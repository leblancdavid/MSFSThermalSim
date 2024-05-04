using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public class CounterBasedTurbulenceModel : ITurbulenceModel
    {
        private readonly ValueRangeInt _framesBetweenTurbulence;
        private readonly ValueRangeInt _turbulenceDuration;
        private readonly ValueRangeDouble _turbulenceStrength;


        private int _counter;
        private int _resetCount;
        private int _duration;
        private double _maxTurbulence;

        private double[] _smoothingKernel;

        private Random _random = new Random();
        
        private TurbulenceEffect? _turbulence = null;

        private double x_scaler = 0.5;
        private double y_scaler = 0.25;
        private double z_scaler = 1.0;


        public CounterBasedTurbulenceModel(ValueRangeInt framesBetweenTurbulence, ValueRangeInt turbulenceDuration, ValueRangeDouble turbulenceStrength)
        {
            _framesBetweenTurbulence = framesBetweenTurbulence;
            _turbulenceDuration = turbulenceDuration;
            _turbulenceStrength = turbulenceStrength;
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
                RotationAccelerationBodyX = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * x_scaler,
                RotationAccelerationBodyY = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * y_scaler,
                RotationAccelerationBodyZ = (2.0 * _random.NextDouble() - 1.0) * _maxTurbulence * z_scaler,
            };

            return _turbulence;
        }

        private void ResetCount()
        {
            _resetCount = _framesBetweenTurbulence.GetRandomValue(_random);
            _duration = _turbulenceDuration.GetRandomValue(_random);
            _maxTurbulence = _turbulenceStrength.GetRandomValue(_random);
            _counter = 0;
        }
    }
}
