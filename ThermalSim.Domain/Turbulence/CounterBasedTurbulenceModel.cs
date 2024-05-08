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
        private readonly ITurbulenceKernel _turbulenceKernel;

        public TurbulenceProperties Properties { get; set; }

        public CounterBasedTurbulenceModel(TurbulenceProperties properties, ITurbulenceKernel? turbulenceKernel = null)
        {

            Properties = properties;
            if(turbulenceKernel != null)
            {
                _turbulenceKernel = turbulenceKernel;
            }
            else
            {
                _turbulenceKernel = new CosineTurbulenceKernel();
            }
        }

        private void UpdateTurbulenceKernel(int duration)
        {
            _smoothingKernel = _turbulenceKernel.GetTurbulenceKernel(duration);
        }

        public TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position, IThermalModel? thermal = null)
        {
            //If we hit turbulence, this will be not null
            if(_turbulence != null)
            {
                _counter++;
                //If we've hit the number of frames of turbulence, then we reset it
                if(_counter >= _duration)
                {
                    _turbulence = null;
                    //ResetCount();
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

            UpdateTurbulenceKernel(_duration);

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
