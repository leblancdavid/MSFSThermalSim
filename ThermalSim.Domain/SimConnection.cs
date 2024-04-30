using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using System.Reflection;

namespace ThermalSim.Domain
{
    public class SimConnection : ISimConnection, IDisposable
    {
        private IntPtr handle;
        private readonly ILogger<SimConnection> logger;

        public bool IsConnected => Connection != null;

        public SimConnect? Connection { get; private set; }


        public SimConnection(ILogger<SimConnection> logger)
        {
            Connection = null;
            this.logger = logger;
        }

        public void Connect()
        {
            Connection = new SimConnect("", handle, ConnectionConstants.WM_USER_SIMCONNECT, null, 0);

            Connection.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Connection_OnRecvOpen);
            Connection.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Connection_OnRecvQuit);

            //simconnect.OnRecvException += Simconnect_OnRecvException;
            //simconnect.OnRecvEvent += Simconnect_OnRecvEvent;
            //simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;
            //RegisterSimStateDefinition();
            //RegisterAircraftPositionDefinition();
            //RegisterAircraftPositionSetDefinition();
            //simconnect.AddToDataDefinition(DEFINITIONS.AircraftPositionInitial, "Initial Position", null, SIMCONNECT_DATATYPE.INITPOSITION, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            //simconnect.OnRecvEventFrame += Simconnect_OnRecvEventFrame;

            //simconnect.SubscribeToSystemEvent(EVENTS.POSITION_CHANGED, "PositionChanged");
            //simconnect.SubscribeToSystemEvent(EVENTS.FRAME, "Frame");
            //simconnect.OnRecvAssignedObjectId += Simconnect_OnRecvAssignedObjectId;

            //simconnect.OnRecvSystemState += Simconnect_OnRecvSystemState;

            //simconnect.MapClientEventToSimEvent(EVENTS.FREEZE_LATITUDE_LONGITUDE, "FREEZE_LATITUDE_LONGITUDE_SET");
            //simconnect.MapClientEventToSimEvent(EVENTS.FREEZE_ALTITUDE, "FREEZE_ALTITUDE_SET");
            //simconnect.MapClientEventToSimEvent(EVENTS.FREEZE_ATTITUDE, "FREEZE_ATTITUDE_SET");
            //RegisterEvents();
        }

        public void Disconnect()
        {
            try
            {
                Connection?.Dispose();
                Connection = null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Cannot unsubscribe events! Error: {ex.Message}");
            }
        }

        private void Connection_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            logger.LogInformation("Connected to Flight Simulator {applicationName}", data.szApplicationName);
            RequestDataOnConnected();
        }

        private void Connection_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            logger.LogInformation("Flight Simulator has exited");
            Disconnect();
        }

        private void RequestDataOnConnected()
        {
            //Is this needed
            //Connection?.RequestDataOnSimObject(
            //    DATA_REQUESTS.SIM_STATE, DEFINITIONS.SimState, 0,
            //    SIMCONNECT_PERIOD.SECOND,
            //    SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT,
            //    0, 0, 0);

            //Connection?.RequestDataOnSimObject(
            //    DATA_REQUESTS.AIRCRAFT_POSITION, DEFINITIONS.AircraftPosition, 0,
            //    SIMCONNECT_PERIOD.SIM_FRAME,
            //    SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT,
            //    0, 0, 0);
        }


        public void Dispose()
        {
            Disconnect();
        }
    }
}
