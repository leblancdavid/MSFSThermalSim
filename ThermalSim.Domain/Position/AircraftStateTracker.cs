using Microsoft.Extensions.Logging;

namespace ThermalSim.Domain.Position
{
    public class AircraftStateTracker : IAircraftStateTracker
    {
        public AircraftStateTracker(ILogger logger, int samplingLength = 20)
        {
            this.logger = logger;
            SamplingLength = samplingLength;
        }

        public int SamplingLength { get; set; }

        public AircraftPositionState? LastState => stateQueue.LastOrDefault();
        public AircraftPositionState? FirstState => stateQueue.FirstOrDefault();

        private AircraftStateChangeInfo? _aircraftStateChangeInfo = null;
        public AircraftStateChangeInfo? AircraftStateChangeInfo => _aircraftStateChangeInfo;

        private Queue<AircraftPositionState> stateQueue = new Queue<AircraftPositionState>();
        private readonly ILogger logger;

        public void UpdatePosition(AircraftPositionState state)
        {
            if(stateQueue.Count >= SamplingLength)
            {
                stateQueue.Dequeue();
            }

            stateQueue.Enqueue(state);

            if(_aircraftStateChangeInfo == null)
            {
                _aircraftStateChangeInfo = new AircraftStateChangeInfo(logger);
                {
                    
                };
            }

            _aircraftStateChangeInfo.AltitudeChange = LastState!.Value.Altitude - FirstState!.Value.Altitude;
            _aircraftStateChangeInfo.AverageAltitude = stateQueue.Average(x => x.Altitude);
            _aircraftStateChangeInfo.AverageVerticalVelocity = (LastState.Value.Altitude - FirstState.Value.Altitude) / (LastState.Value.AbsoluteTime - FirstState.Value.AbsoluteTime);
            _aircraftStateChangeInfo.AverageVelocity = stateQueue.Average(x => GetVelocity(x));
        }

        private double GetVelocity(AircraftPositionState state)
        {
            return Math.Sqrt(state.VelocityBodyX * state.VelocityBodyX + state.VelocityBodyY * state.VelocityBodyY + state.VelocityBodyZ * state.VelocityBodyZ);
        }
    }
}
