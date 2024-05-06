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
        public double TowingSpeed { get; set; } = 5.0;

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

                var rotationFactor = -2.0;
                var rudderFactor = 50.0;

                var speed = TowingSpeed * (1.0 - e.Position.SpoilerHandlePosition / 100.0);
                var rotation = rotationFactor * e.Position.Bank;
                var rudder = rudderFactor * e.Position.RudderPosition;
                var update = new TowingSpeedUpdate()
                {
                    RotationAccelerationBodyY = rudder,
                    RotationAccelerationBodyZ = rotation,
                    VelocityBodyZ = speed,
                };
                //e.Position.ThrottleLeverPosition1 = e.Position.ThrottleLeverPosition2;

                connection.Connection?.SetDataOnSimObject(SimDataEventTypes.TowingSpeedUpdate,
                            1u, SIMCONNECT_DATA_SET_FLAG.DEFAULT, update);
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
