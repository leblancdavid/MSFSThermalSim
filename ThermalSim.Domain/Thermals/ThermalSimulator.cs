using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using System.Net.WebSockets;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Extensions;
using ThermalSim.Domain.Notifications;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulator : IThermalSimulator, IDisposable
    {
        private readonly ISimConnection connection;
        private readonly IThermalGenerator thermalGenerator;
        private readonly IEventNotifier<WebSocket> eventNotifier;
        private readonly ILogger<ThermalSimulator> logger;
        private AircraftStateTracker stateTracker;

        private DateTime nextSampleTime = DateTime.Now;

        private List<IThermalModel> thermals = new List<IThermalModel>();

        public ThermalSimulator(ISimConnection connection,
            IThermalGenerator thermalGenerator,
            IEventNotifier<WebSocket> eventNotifier,
            ILogger<ThermalSimulator> logger)
        {
            this.connection = connection;
            this.thermalGenerator = thermalGenerator;
            this.eventNotifier = eventNotifier;
            this.logger = logger;
            this.stateTracker = new AircraftStateTracker(logger);
        }

        public bool IsRunning { get; private set; }
        public ThermalSimulationConfiguration Configuration
        {
            get
            {
                return thermalGenerator.Configuration;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                thermalGenerator.Configuration = value;
            }
        }
        public void Dispose()
        {
            Stop();
        }

        public bool Start()
        {
            if(!connection.IsConnected)
            {
                if(IsRunning)
                {
                    Stop();
                }

                var result = connection.Connect();
                if (!result)
                    return result;
            }

            if (!IsRunning)
            {
                connection.AircraftPositionUpdated += OnAircraftPositionUpdate;
                IsRunning = true;

                connection.Connection?.SetDataOnSimObject(
                    SimDataEventTypes.ThermalSimEnableFlag,
                    1u, SIMCONNECT_DATA_SET_FLAG.DEFAULT, 
                    new ThermalSimEnabled() { ThermalSimIsEnabled = 1.0 });
            }
            
            
            return IsRunning;
        }

        private void OnAircraftPositionUpdate(object? sender, AircraftPositionUpdatedEventArgs e)
        {
            try
            {
                stateTracker.UpdatePosition(e.Position);

                if (!IsRunning)
                    return;

                if (DateTime.Now > nextSampleTime)
                {
                    nextSampleTime = thermalGenerator.GetNextSampleTime();
                    UpdateThermalModels(e.Position);
                }

                _ = ApplyThermalEffect(e.Position);

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
                thermals.Remove(t);
                logger.LogInformation($"Thermal removed {t.CalcDistance(position)}ft away: ({t.Properties.Latitude},{t.Properties.Longitude}) at {t.Properties.Altitude}ft. (count: {thermals.Count})");
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
                var isNearAnotherObject = true;
                IThermalModel? newThermalModel = null;
                int maxTry = 100;
                int itr = 0;
                //This loop ensures that we don't spawn thermals inside existing ones
                do
                {
                    newThermalModel = thermalGenerator.GenerateThermalAroundAircraft(position);
                    isNearAnotherObject = thermals.Any(x => 
                    x.CalcDistance(newThermalModel.Properties.Latitude,
                        newThermalModel.Properties.Longitude) < x.Properties.TotalRadius + newThermalModel.Properties.TotalRadius) ||
                        (!Configuration.AllowSpawningOnAircraft && newThermalModel.IsInThermal(position));
                    itr++;
                }
                while (isNearAnotherObject && itr < maxTry);

                if (newThermalModel != null)
                {
                    double distanceToAircraft = newThermalModel.CalcDistance(position);
                    thermals.Add(newThermalModel);

                    logger.LogInformation($"Thermal created (Total Count: {thermals.Count}) \n\tDistance: {distanceToAircraft}ft away \n\tCoordinates: ({newThermalModel.Properties.Latitude},{newThermalModel.Properties.Longitude}) \n\tAltitude: {newThermalModel.Properties.Altitude}ft. \n\tRadius: {newThermalModel.Properties.TotalRadius}ft. \n\tCore Rate: {newThermalModel.Properties.CoreLiftRate}ft/s");
                }
            } //Repeat if we have less than the minimum number
            while (thermals.Count < thermalGenerator.Configuration.NumberOfThermals.Min);
        }
        
        private async Task ApplyThermalEffect(AircraftPositionState position)
        {
            try
            {
                thermals.ForEach(x => x.ApplyWindDrift(position.WindDirection, position.WindVelocity));

                double minDistance = double.MaxValue;
                IThermalModel? nearestThermal = null;
                foreach (var t in thermals)
                {
                    double d = t.CalcDistance(position);
                    if(d < minDistance && 
                        position.Altitude >= t.Properties.Altitude &&
                        position.Altitude <= t.Properties.TopAltitude)
                    {
                        minDistance = d;
                        nearestThermal = t;
                    }
                }

                var thermalEvent = new AircraftThermalPositionEvent();
                thermalEvent.NearestThermalDistance = nearestThermal == null ? 0.0 : minDistance;
                thermalEvent.RelativeNearestThermal = nearestThermal == null ? 0.0 : nearestThermal.GetRelativeDirection(position);
                thermalEvent.WindHeading = position.WindDirection;
                thermalEvent.WindSpeed = position.WindVelocity;
                thermalEvent.AircraftHeading = position.HeadingIndicator;

                //If we are not in a thermal, don't do anything
                if (nearestThermal == null || !nearestThermal.IsInThermal(position))
                {
                    if(stateTracker.AircraftStateChangeInfo != null)
                        stateTracker.AircraftStateChangeInfo.ThermalState = ThermalPositionState.NotInThermal;

                    thermalEvent.ThermalState = ThermalPositionState.NotInThermal;
                    
                    await eventNotifier.NotifyAsync(thermalEvent);

                    return;
                }

                var velocityChange = nearestThermal.GetThermalAltitudeChange(position, stateTracker.AircraftStateChangeInfo);

                thermalEvent.ThermalState = stateTracker.AircraftStateChangeInfo == null ? ThermalPositionState.NotInThermal : stateTracker.AircraftStateChangeInfo.ThermalState;
                thermalEvent.CurrentLift = stateTracker.AircraftStateChangeInfo == null ? 0.0 : stateTracker.AircraftStateChangeInfo.BaseLiftValue;

                if (velocityChange != null)
                {
                    connection.Connection?.SetDataOnSimObject(SimDataEventTypes.ThermalVelocityUpdate,
                        1u, SIMCONNECT_DATA_SET_FLAG.DEFAULT, velocityChange);

                    var turbulence = nearestThermal.TurbulenceModel.GetTurbulenceEffect(position, nearestThermal);
                    if (turbulence != null)
                    {
                        //Console.WriteLine($"Turbulence: ({turbulence.Value.RotationAccelerationBodyX}, {turbulence.Value.RotationAccelerationBodyY}, {turbulence.Value.RotationAccelerationBodyZ})");
                        connection.Connection?.SetDataOnSimObject(SimDataEventTypes.TurbulenceEffect,
                            1u, SIMCONNECT_DATA_SET_FLAG.DEFAULT, turbulence);
                    }
                }

                await eventNotifier.NotifyAsync(thermalEvent);
                
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Unable to apply thermal effect {ex.Message}");
            }
        }

        public void Stop()
        {
            IsRunning = false;
            connection.AircraftPositionUpdated -= OnAircraftPositionUpdate;

            connection.Connection?.SetDataOnSimObject(
                    SimDataEventTypes.ThermalSimEnableFlag,
                    1u, SIMCONNECT_DATA_SET_FLAG.DEFAULT,
                    new ThermalSimEnabled() { ThermalSimIsEnabled = 0.0 });
        }

        public bool InsertThermal()
        {
            if(!IsRunning || stateTracker.LastState == null)
            {
                return false;
            }

            var t = thermalGenerator.GenerateThermalAtAircraft(stateTracker.LastState.Value);
            thermals.Add(t);

            return true;
        }

        private void DebugTrace(AircraftPositionState position, double distanceToNearest, bool inThermal)
        {
            string thermalMsg = inThermal ? "(IN THERMAL)" : "";
            logger.LogInformation($"Aircraft: ({position.Latitude},{position.Longitude}) at {position.Altitude}ft. Nearest Thermal: {distanceToNearest}ft {thermalMsg}");
        }
    }
}
