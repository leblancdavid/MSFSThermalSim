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

        private const double ROTATION_FACTOR = -0.05;
        private const double RUDDER_FACTOR = 0.25;
        private const double STOP_ROTATION_THRESHOLD = 0.05;

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

                var rotation = ROTATION_FACTOR * e.Position.Bank;
                var rudder = RUDDER_FACTOR * e.Position.RudderPosition;

                var speed = TowingSpeed * (1.0 - e.Position.SpoilerHandlePosition / 100.0);

                //Stop towing if the the wing is being lifted
                if(Math.Abs(rotation) > STOP_ROTATION_THRESHOLD)
                {
                    speed = 0.0;
                }

                var update = new TowingSpeedUpdate()
                {
                    RotationVelocityBodyY = rudder,
                    RotationVelocityBodyZ = rotation,
                    VelocityBodyZ = speed,
                };

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
