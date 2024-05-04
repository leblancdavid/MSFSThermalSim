using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Turbulence
{
    public class CounterBasedTurbulenceModel : ITurbulenceModel
    {
        private int _minCount;
        private int _maxCount;
        private int _counter;
        private int _resetCount;
        private double _maxTurbulence;
        private Random _random = new Random();

        public CounterBasedTurbulenceModel(int minCount, int maxCount, double maxTurbulence)
        {
            _minCount = minCount;
            _maxCount = maxCount;
            _maxTurbulence = maxTurbulence;
            ResetCount();
        }

        public TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position)
        {
            if(_counter < _resetCount)
            {
                _counter++;
                return null;
            }

            ResetCount();

            //For now let's make it simple
            return new TurbulenceEffect()
            {
                RotationAccelerationBodyX = _random.NextDouble() * _maxTurbulence,
                RotationAccelerationBodyY = _random.NextDouble() * _maxTurbulence,
                RotationAccelerationBodyZ = _random.NextDouble() * _maxTurbulence,
            };
        }

        private void ResetCount()
        {
            _resetCount = _random.Next(_minCount, _maxCount);
            _counter = 0;
        }
    }
}
