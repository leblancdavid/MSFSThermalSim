using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Extensions;
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
                    nextSampleTime = thermalGenerator.GetNextSampleTime();
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
            //Removed all thermals that have expired
            RemoveExpiredThermals();

            //Replace thermals that are out of range
            ReplaceFarAwayThermals(position);

            //Create new thermals
            CreateNewThermals(position);
        }

        private void RemoveExpiredThermals()
        {
            var currentTime = DateTime.Now;
             
            //var expiredThermals = thermals.Where(x => x.Properties.EndTime < currentTime);
            //foreach (var thermal in expiredThermals)
            //{
            //    Console.WriteLine($"Thermal expired: ({thermal.Properties.Latitude},{thermal.Properties.Longitude}) at {thermal.Properties.Altitude}ft.");
            //}

            thermals.RemoveAll(x => x.Properties.EndTime < currentTime);
        }

        private void ReplaceFarAwayThermals(AircraftPositionState position)
        {
            var distantThermals = thermals.Where(x => x.CalcDistance(position) > thermalGenerator.Configuration.ReplaceDistance).ToList();
            foreach(var t in distantThermals)
            {
                //Console.WriteLine($"Thermal removed {t.CalcDistance(position)}ft away: ({t.Properties.Latitude},{t.Properties.Longitude}) at {t.Properties.Altitude}ft.");

                thermals.Remove(t);
                CreateNewThermals(position);
            }

        }

        private void CreateNewThermals(AircraftPositionState position)
        {
            //If we have reached the max number of thermals, ignore
            if (thermals.Count >= thermalGenerator.Configuration.NumberOfThermals.Max ||
                !connection.IsConnected)
            {
                return;
            }

            do
            {
                //Generate a new thermal
                var isNearAnotherThermal = true;
                IThermalModel? newThermalModel = null;
                int maxTry = 100;
                int itr = 0;
                //This loop ensures that we don't spawn thermals inside existing ones
                do
                {
                    newThermalModel = thermalGenerator.GenerateThermalAroundAircraft(position);
                    isNearAnotherThermal = thermals.Any(x => x.IsInThermal(
                        newThermalModel.Properties.Latitude,
                        newThermalModel.Properties.Longitude,
                        newThermalModel.Properties.Altitude));
                    itr++;
                }
                while (isNearAnotherThermal && itr < maxTry);

                if (newThermalModel != null)
                {
                    //double distanceToAircraft = newThermalModel.CalcDistance(position);
                    //Console.WriteLine($"Thermal created {distanceToAircraft}ft away: ({newThermalModel.Properties.Latitude},{newThermalModel.Properties.Longitude}) at {newThermalModel.Properties.Altitude}ft.");
                    
                    thermals.Add(newThermalModel);
                }
            } //Repeat if we have less than the minimum number
            while (thermals.Count < thermalGenerator.Configuration.NumberOfThermals.Min);
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
                    if (d < minDistance)
                    {
                        minDistance = d;
                        if(t.IsInThermal(position))
                            nearestThermal = t;
                    }
                }

                //DebugTrace(position, minDistance, nearestThermal != null);

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

        private void DebugTrace(AircraftPositionState position, double distanceToNearest, bool inThermal)
        {
            string thermalMsg = inThermal ? "(IN THERMAL)" : "";
            Console.WriteLine($"Aircraft: ({position.Latitude},{position.Longitude}) at {position.Altitude}ft. Nearest Thermal: {distanceToNearest}ft {thermalMsg}");
        }
    }
}
