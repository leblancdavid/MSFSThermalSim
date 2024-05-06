using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Towing
{
    public class TowingService : ITowingService
    {
        private readonly ISimConnection connection;
        private readonly ILogger<TowingService> logger;

        public TowingService(ISimConnection connection, ILogger<TowingService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public bool IsTowing { get; private set; }
        public double TowingSpeed { get; set; }

        public bool StartTowing()
        {
            if (!connection.IsConnected)
            {
                var result = connection.Connect();
                if(!result)
                {
                    logger.LogWarning("Unable to connect to the simulator");
                    return false;
                }
            }

            IsTowing = true;

            connection.AircraftPositionUpdated += OnAircraftPositionUpdate;

            return true;
        }

        private void OnAircraftPositionUpdate(object? sender, AircraftPositionUpdatedEventArgs e)
        {
            try
            {
                if (!IsTowing || e.Position.IsOnGround == 0 || e.Position.GearHandlePosition == 0)
                {
                    return;
                }

                //var speed = ;
                //e.Position.ThrottleLeverPosition1 = e.Position.ThrottleLeverPosition2;

                //connection.Connection?.SetDataOnSimObject(SimDataEventTypes.TurbulenceEffect,
                //            1u, SIMCONNECT_DATA_SET_FLAG.DEFAULT, turbulence);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while updating the towing status: {ex.Message}");
            }
        }

        public bool StopTowing()
        {
            IsTowing = false;
            connection.AircraftPositionUpdated -= OnAircraftPositionUpdate;
            return true;
        }
    }
}
