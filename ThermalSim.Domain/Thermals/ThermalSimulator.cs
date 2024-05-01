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

        private List<Thermal> thermals = new List<Thermal>();

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
                    ProcessThermals(e.Position);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while processing thermals: {ex.Message}");
            }
        }

        private void ProcessThermals(AircraftPositionState position)
        {
            RemoveExpiredThermals();
            GenerateNewThermals(position);
        }

        private void RemoveExpiredThermals()
        {
            var currentTime = DateTime.Now;
            var expiredThermals = thermals.Where(x => x.EndTime < currentTime);
            
            foreach(var t in expiredThermals)
            {
                connection.Connection?.WeatherRemoveThermal(t.ObjectId);
            }

            thermals.RemoveAll(x => x.EndTime > currentTime);
        }

        private void GenerateNewThermals(AircraftPositionState position)
        {
            if (thermals.Count >= configuration.MaxNumberOfThermals ||
                !connection.IsConnected)
            {
                return;
            }

            while(thermals.Count < configuration.MinNumberOfThermals)
            {
                var t = thermalGenerator.GenerateThermalAroundAircraft(position);
                connection.Connection?.WeatherCreateThermal(SimDataEventTypes.NewThermal,
                    t.Latitude,
                    t.Longitude,
                    t.Altitude,
                    t.Radius,
                    t.Height,
                    t.CoreRate,
                    t.CoreTurbulence,
                    t.SinkRate,
                    t.SinkTurbulence,
                    t.CoreSize,
                    t.CoreTransitionSize,
                    t.SinkLayerSize,
                    t.SinkTransitionSize);
            }
        }

        private void RemoveThermal(Thermal t)
        {

            thermals.Remove(t);
        }

        public void Stop()
        {
            connection.AircraftPositionUpdated -= OnAircraftPositionUpdate;
            connection.Disconnect();
        }
    }
}
