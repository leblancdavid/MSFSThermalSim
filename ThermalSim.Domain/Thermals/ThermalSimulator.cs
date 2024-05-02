using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulator : IThermalSimulator, IDisposable
    {
        private readonly ISimConnection connection;
        private readonly IThermalGenerator thermalGenerator;
        private readonly ILogger<ThermalSimulator> logger;
        private AircraftStateTracker stateTracker = new AircraftStateTracker();

        private DateTime nextSampleTime = DateTime.Now;
        private ThermalSimulationConfiguration configuration = new ThermalSimulationConfiguration();

        private List<IThermalModel> thermals = new List<IThermalModel>();

        public ThermalSimulator(ISimConnection connection,
            IThermalGenerator thermalGenerator,
            ILogger<ThermalSimulator> logger)
        {
            this.connection = connection;
            this.thermalGenerator = thermalGenerator;
            this.logger = logger;
        }

        public bool IsRunning { get; private set; }

        public void Dispose()
        {
            Stop();
        }

        public bool Start()
        {
            if(connection.IsConnected)
            {
                IsRunning = true;
            }
            else
            {
                var result = connection.Connect();
                if (result)
                {
                    connection.AircraftPositionUpdated += OnAircraftPositionUpdate;
                    IsRunning = true;
                }
            }
            
            return IsRunning;
        }

        private void OnAircraftPositionUpdate(object? sender, AircraftPositionUpdatedEventArgs e)
        {
            try
            {
                stateTracker.UpdatePosition(e.Position);

                if (DateTime.Now > nextSampleTime)
                {
                    nextSampleTime = DateTime.Now + configuration.SamplingSpeed;
                    UpdateThermalModels(e.Position);
                }

                ApplyThermalEffect(e.Position);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while processing thermals: {ex.Message}");
            }
        }

        private void UpdateThermalModels(AircraftPositionState position)
        {
            //Remove any thermals that have expired
            var currentTime = DateTime.Now;
            thermals.RemoveAll(x => x.EndTime > currentTime);

            //If we have reached the max number of thermals, ignore
            if(thermals.Count >= configuration.MaxNumberOfThermals ||
                !connection.IsConnected)
            {
                return;
            }

            do
            {
                //Generate a new thermal
                var t = thermalGenerator.GenerateThermalAroundAircraft(position);
                thermals.Add(t);
            } //Repeat if we have less than the minimum number
            while (thermals.Count < configuration.MinNumberOfThermals);
        }

        private void ApplyThermalEffect(AircraftPositionState position)
        {
            try
            {
                double minDistance = double.MaxValue;
                IThermalModel? nearestThermal = null;
                foreach (var t in thermals)
                {
                    var d = t.GetDistanceToThermal(position);
                    if (d < minDistance && t.IsInThermal(position))
                    {
                        minDistance = d;
                        nearestThermal = t;
                    }
                }

                //If we are not in a thermal, don't do anything
                if (nearestThermal == null)
                {
                    return;
                }

                var velocityChange = nearestThermal.GetThermalAltitudeChange(position, stateTracker.AircraftStateChangeInfo);

                if(velocityChange != null)
                {
                    connection.Connection?.SetDataOnSimObject(SimDataEventTypes.ThermalVelocityUpdate,
                        1u, SIMCONNECT_DATA_SET_FLAG.DEFAULT, velocityChange);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Unable to apply thermal effect {ex.Message}");
            }
        }

        public void Stop()
        {
            connection.AircraftPositionUpdated -= OnAircraftPositionUpdate;
            connection.Disconnect();
        }

        public bool InsertThermal()
        {
            if(!IsRunning || stateTracker.LastState == null)
            {
                return false;
            }    
            var t = thermalGenerator.GenerateThermalAroundAircraft(stateTracker.LastState.Value);
            thermals.Add(t);

            return true;
        }
    }
}
