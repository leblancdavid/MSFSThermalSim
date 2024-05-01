using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using System.Net;
using System.Windows.Forms;
using ThermalSim.Domain.Position;
using ThermalSim.Helpers;

namespace ThermalSim.Domain.Connection
{
    public class SimConnection : ISimConnection, IDisposable
    {
        private readonly ILogger<SimConnection> logger;
        private MessageHandler messageHandler = new MessageHandler();
        private static CancellationTokenSource? source = null;
        private static CancellationToken token = CancellationToken.None;
        private static Task? messagePump;
        private static AutoResetEvent messagePumpRunning = new AutoResetEvent(false);
        // User-defined win32 event
        const int WM_USER_SIMCONNECT = 0x0402;
        public bool IsConnected => Connection != null;

        public SimConnect? Connection { get; private set; }
        public event EventHandler<AircraftPositionUpdatedEventArgs>? AircraftPositionUpdated;


        public SimConnection(ILogger<SimConnection> logger)
        {
            Connection = null;
            this.logger = logger;
        }

        public bool Connect()
        {
            try
            {
                if (Connection != null)
                {
                    Disconnect();
                };

                source = new CancellationTokenSource();
                token = source.Token;
                token.ThrowIfCancellationRequested();
                messagePump = new Task(ConnectThread, token);
                messagePump.Start();
                messagePumpRunning = new AutoResetEvent(false);
                messagePumpRunning.WaitOne();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Unable to connect to SimConnect! Error: {ex.Message}");
                return false;
            }
        }

        private void ConnectThread()
        {
            messageHandler = new MessageHandler();
            messageHandler.MessageReceived += MessageHandler_MessageReceived;
            messageHandler.CreateHandle();

            Connection = new SimConnect("ThermalSim", messageHandler.Handle, ConnectionConstants.WM_USER_SIMCONNECT, null, 0);

            Connection.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Connection_OnRecvOpen);
            Connection.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Connection_OnRecvQuit);

            Connection.OnRecvException += Connection_OnRecvException;
            Connection.OnRecvEvent += Connection_OnRecvEvent;
            Connection.OnRecvSimobjectData += Connection_OnRecvSimobjectData;

            RegisterAircraftPositionDefinition();
            //RegisterNewThermalDefinition();

            messagePumpRunning.Set();
            Application.Run();
        }

        private void MessageHandler_MessageReceived(object? sender, System.Windows.Forms.Message e)
        {
            if (e.Msg == WM_USER_SIMCONNECT && Connection != null)
            {
                try
                {
                    Connection.ReceiveMessage();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred while handling message: {ex.Message}");

                }

            }
        }

        public void Disconnect()
        {
            try
            {
                if (source != null && token.CanBeCanceled)
                {
                    source.Cancel();
                }
                if (messagePump != null)
                {
                    messageHandler.MessageReceived -= MessageHandler_MessageReceived;
                    messageHandler.DestroyHandle();
                    messageHandler = null;

                    messagePumpRunning.Close();
                    messagePumpRunning.Dispose();
                }
                messagePump = null;

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
        private void Connection_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
            logger.LogDebug("OnRecvEvent dwID {dwID} uEventID {uEventID}", (SIMCONNECT_RECV_ID)data.dwID, data.uEventID);
        }

        private void Connection_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            var error = (SIMCONNECT_EXCEPTION)data.dwException;
            logger.LogError("SimConnect error received: {error}", error);
        }

        private void Connection_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            if (data.dwRequestID == (uint)SimDataRequests.AIRCRAFT_POSITION)
            {
                var position = data.dwData[0] as AircraftPositionState?;
                if (position != null)
                {
                    AircraftPositionUpdated?.Invoke(this, new AircraftPositionUpdatedEventArgs(position.Value));
                }
            }
        }

        private void RequestDataOnConnected()
        {
            Connection?.RequestDataOnSimObject(
                SimDataRequests.AIRCRAFT_POSITION,
                SimDataEventTypes.AircraftPosition, 0,
                SIMCONNECT_PERIOD.SECOND,
                SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT,
                0, 0, 0);
        }

        private void RegisterAircraftPositionDefinition()
        {
            RegisterDataDefinition<AircraftPositionState>(SimDataEventTypes.AircraftPosition,
                ("PLANE LATITUDE", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("PLANE LONGITUDE", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("PLANE ALTITUDE", "Feet", (SIMCONNECT_DATATYPE)4),
                ("PLANE PITCH DEGREES", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("PLANE BANK DEGREES", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("PLANE HEADING DEGREES GYRO", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("PLANE HEADING DEGREES TRUE", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("PLANE HEADING DEGREES MAGNETIC", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("VELOCITY BODY X", "Feet per second", (SIMCONNECT_DATATYPE)4),
                ("VELOCITY BODY Y", "Feet per second", (SIMCONNECT_DATATYPE)4),
                ("VELOCITY BODY Z", "Feet per second", (SIMCONNECT_DATATYPE)4),
                ("AILERON POSITION", "Position", (SIMCONNECT_DATATYPE)4),
                ("ELEVATOR POSITION", "Position", (SIMCONNECT_DATATYPE)4),
                ("RUDDER POSITION", "Position", (SIMCONNECT_DATATYPE)4),
                ("ELEVATOR TRIM POSITION", "Radians", (SIMCONNECT_DATATYPE)4),
                ("AILERON TRIM PCT", "Percent Over 100", (SIMCONNECT_DATATYPE)4),
                ("RUDDER TRIM PCT", "Percent Over 100", (SIMCONNECT_DATATYPE)4),
                ("FLAPS HANDLE INDEX", "Number", (SIMCONNECT_DATATYPE)1),
                ("TRAILING EDGE FLAPS LEFT PERCENT", "Position", (SIMCONNECT_DATATYPE)4),
                ("TRAILING EDGE FLAPS RIGHT PERCENT", "Position", (SIMCONNECT_DATATYPE)4),
                ("LEADING EDGE FLAPS LEFT PERCENT", "Position", (SIMCONNECT_DATATYPE)4),
                ("LEADING EDGE FLAPS RIGHT PERCENT", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG THROTTLE LEVER POSITION:1", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG THROTTLE LEVER POSITION:2", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG THROTTLE LEVER POSITION:3", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG THROTTLE LEVER POSITION:4", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG PROPELLER LEVER POSITION:1", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG PROPELLER LEVER POSITION:2", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG PROPELLER LEVER POSITION:3", "Position", (SIMCONNECT_DATATYPE)4),
                ("GENERAL ENG PROPELLER LEVER POSITION:4", "Position", (SIMCONNECT_DATATYPE)4),
                ("SPOILERS HANDLE POSITION", "Percent", (SIMCONNECT_DATATYPE)4),
                ("GEAR HANDLE POSITION", "Bool", (SIMCONNECT_DATATYPE)1),
                ("WATER RUDDER HANDLE POSITION", "Percent Over 100", (SIMCONNECT_DATATYPE)4),
                ("BRAKE LEFT POSITION", "Position", (SIMCONNECT_DATATYPE)4),
                ("BRAKE RIGHT POSITION", "Position", (SIMCONNECT_DATATYPE)4),
                ("BRAKE PARKING POSITION", "Position", (SIMCONNECT_DATATYPE)1),
                ("LIGHT TAXI", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT LANDING", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT STROBE", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT BEACON", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT NAV", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT WING", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT LOGO", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT RECOGNITION", "Bool", (SIMCONNECT_DATATYPE)1),
                ("LIGHT CABIN", "Bool", (SIMCONNECT_DATATYPE)1),
                ("SIMULATION RATE", "Number", (SIMCONNECT_DATATYPE)1),
                ("ABSOLUTE TIME", "Seconds", (SIMCONNECT_DATATYPE)4),
                ("PLANE ALT ABOVE GROUND", "Feet", (SIMCONNECT_DATATYPE)4),
                ("SIM ON GROUND", "Bool", (SIMCONNECT_DATATYPE)1),
                ("AMBIENT WIND VELOCITY", "Knots", (SIMCONNECT_DATATYPE)4),
                ("AMBIENT WIND DIRECTION", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("G FORCE", "GForce", (SIMCONNECT_DATATYPE)4),
                ("PLANE TOUCHDOWN NORMAL VELOCITY", "Feet per minute", (SIMCONNECT_DATATYPE)4),
                ("WING FLEX PCT:1", "Percent over 100", (SIMCONNECT_DATATYPE)4),
                ("WING FLEX PCT:2", "Percent over 100", (SIMCONNECT_DATATYPE)4),
                ("WING FLEX PCT:3", "Percent over 100", (SIMCONNECT_DATATYPE)4),
                ("WING FLEX PCT:4", "Percent over 100", (SIMCONNECT_DATATYPE)4),
                ("AIRSPEED TRUE", "Knots", (SIMCONNECT_DATATYPE)4),
                ("AIRSPEED INDICATED", "Knots", (SIMCONNECT_DATATYPE)4),
                ("AIRSPEED MACH", "Mach", (SIMCONNECT_DATATYPE)4),
                ("GPS GROUND SPEED", "Knots", (SIMCONNECT_DATATYPE)4),
                ("GROUND VELOCITY", "Knots", (SIMCONNECT_DATATYPE)4),
                ("HEADING INDICATOR", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("ATTITUDE INDICATOR PITCH DEGREES", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("ATTITUDE INDICATOR BANK DEGREES", "Degrees", (SIMCONNECT_DATATYPE)4),
                ("RECIP ENG MANIFOLD PRESSURE:1", "Psi", (SIMCONNECT_DATATYPE)4),
                ("RECIP ENG MANIFOLD PRESSURE:2", "Psi", (SIMCONNECT_DATATYPE)4),
                ("RECIP ENG MANIFOLD PRESSURE:3", "Psi", (SIMCONNECT_DATATYPE)4),
                ("RECIP ENG MANIFOLD PRESSURE:4", "Psi", (SIMCONNECT_DATATYPE)4),
                ("TURN COORDINATOR BALL", "Position 128", (SIMCONNECT_DATATYPE)4),
                ("HSI CDI NEEDLE", "Number", (SIMCONNECT_DATATYPE)4),
                ("STALL WARNING", "Bool", (SIMCONNECT_DATATYPE)1),
                ("ROTATION VELOCITY BODY X", "Radians per second", (SIMCONNECT_DATATYPE)4),
                ("ROTATION VELOCITY BODY Y", "Radians per second", (SIMCONNECT_DATATYPE)4),
                ("ROTATION VELOCITY BODY Z", "Radians per second", (SIMCONNECT_DATATYPE)4),
                ("ACCELERATION BODY X", "Feet per second squared", (SIMCONNECT_DATATYPE)4),
                ("ACCELERATION BODY Y", "Feet per second squared", (SIMCONNECT_DATATYPE)4),
                ("ACCELERATION BODY Z", "Feet per second squared", (SIMCONNECT_DATATYPE)4)
            );
        }

        private void RegisterDataDefinition<T>(SimDataEventTypes definition,
            params (string datumName, string? unitsName, SIMCONNECT_DATATYPE datumType)[] data)
        {
            foreach (var (datumName, unitsName, datumType) in data)
            {
                Connection?.AddToDataDefinition(definition, datumName, unitsName, datumType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            }
            Connection?.RegisterDataDefineStruct<T>(definition);
        }
       
        public void Dispose()
        {
            Disconnect();
        }
    }
}
