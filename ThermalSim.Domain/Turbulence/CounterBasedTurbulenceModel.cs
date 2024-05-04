using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public class CounterBasedTurbulenceModel : ITurbulenceModel
    {
        private int _minCount;
        private int _maxCount;
        private int _counter;
        private int _resetCount;
        private Random _random = new Random();

        public CounterBasedTurbulenceModel(int minCount, int maxCount)
        {
            _minCount = minCount;
            _maxCount = maxCount;

            ResetCount();
        }

        public TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position, IThermalModel thermal)
        {
            if(_counter < _resetCount)
            {
                _counter++;
                return null;
            }

            throw new NotImplementedException();
        }

        private void ResetCount()
        {
            _resetCount = _random.Next(_minCount, _maxCount);
            _counter = 0;
        }
    }
}
