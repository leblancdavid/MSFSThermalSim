using Microsoft.Extensions.Logging;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulator : IThermalSimulator, IDisposable
    {
        private readonly ISimConnection connection;
        private readonly IThermalGenerator thermalGenerator;
        private readonly ILogger<ThermalSimulator> logger;

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

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            connection.Connect();
            connection.AircraftPositionUpdated += OnAircraftPositionUpdate;
        }

        private void OnAircraftPositionUpdate(object? sender, AircraftPositionUpdatedEventArgs e)
        {
            try
            {
                if (DateTime.Now > nextSampleTime)
                {
                    nextSampleTime = DateTime.Now + configuration.SamplingSpeed;
                    UpdateThermalModels(e.Position);
                }


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
            float minDistance = float.MaxValue;
            IThermalModel? nearestThermal = null;
            foreach(var t in thermals)
            {
                var d = t.GetDistanceToThermal(position);
                if(d < minDistance && t.IsInThermal(position))
                {
                    minDistance = d;
                    nearestThermal = t;
                }
            }

            //If we are not in a thermal, don't do anything
            if(nearestThermal == null)
            {
                return;
            }

            var velocityChange = nearestThermal.GetThermalVelocity(position);


        }

        public void Stop()
        {
            connection.AircraftPositionUpdated -= OnAircraftPositionUpdate;
            connection.Disconnect();
        }
    }
}
