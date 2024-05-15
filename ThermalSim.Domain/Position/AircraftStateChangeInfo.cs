using Microsoft.Extensions.Logging;

namespace ThermalSim.Domain.Position
{
    public class AircraftStateChangeInfo
    {
        public double AverageAltitude { get; set; }
        public double AltitudeChange { get; set; }
        public double AverageVerticalVelocity { get; set; }
        public double AverageVelocity { get; set; }
        public double BaseLiftValue { get; set; }
        private ThermalPositionState _thermalState;
        private readonly ILogger logger;

        public ThermalPositionState ThermalState
        {
            get
            {
                return _thermalState;
            }
            set
            {
                if (_thermalState != value)
                {
                    logger.LogInformation($"{value} <- {_thermalState}");
                    _thermalState = value;
                }
            }
        }


        public AircraftStateChangeInfo(ILogger logger)
        {
            this.logger = logger;
            this._thermalState = ThermalPositionState.NotInThermal;
        }

    }
}
