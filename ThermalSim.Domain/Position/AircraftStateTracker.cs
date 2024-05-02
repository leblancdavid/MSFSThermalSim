﻿namespace ThermalSim.Domain.Position
{
    public class AircraftStateTracker : IAircraftStateTracker
    {
        public AircraftStateTracker(int samplingLength = 20)
        {
            SamplingLength = samplingLength;
        }

        public int SamplingLength { get; set; }

        public AircraftPositionState? LastState => stateQueue.LastOrDefault();
        public AircraftPositionState? FirstState => stateQueue.FirstOrDefault();

        public AircraftStateChangeInfo? AircraftStateChangeInfo 
        { 
            get
            {
                if(!stateQueue.Any())
                {
                    return null;
                }
                if(stateQueue.Count < SamplingLength && LastState.HasValue) 
                {
                    return new AircraftStateChangeInfo()
                    {
                        AltitudeChange = 0.0,
                        AverageAltitude = LastState.Value.Altitude,
                        AverageVerticalVelocity = LastState.Value.VerticalSpeed,
                        AverageVelocity = GetVelocity(LastState.Value)
                    };
                }

                if(LastState.HasValue && FirstState.HasValue) 
                {
                    return new AircraftStateChangeInfo()
                    {
                        AltitudeChange = LastState.Value.Altitude - FirstState.Value.Altitude,
                        AverageAltitude = stateQueue.Average(x => x.Altitude),
                        AverageVerticalVelocity = (LastState.Value.Altitude - FirstState.Value.Altitude) / (LastState.Value.AbsoluteTime - FirstState.Value.AbsoluteTime),
                        AverageVelocity = stateQueue.Average(x => GetVelocity(x))
                    };
                }

                return null;
            }
        }

        private Queue<AircraftPositionState> stateQueue = new Queue<AircraftPositionState>();
        public void UpdatePosition(AircraftPositionState state)
        {
            if(stateQueue.Count >= SamplingLength)
            {
                stateQueue.Dequeue();
            }

            stateQueue.Enqueue(state);
        }

        private double GetVelocity(AircraftPositionState state)
        {
            return Math.Sqrt(state.VelocityBodyX * state.VelocityBodyX + state.VelocityBodyY * state.VelocityBodyY + state.VelocityBodyZ * state.VelocityBodyZ);
        }
    }
}
