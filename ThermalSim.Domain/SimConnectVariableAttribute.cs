using Microsoft.FlightSimulator.SimConnect;

//Credit: FlightRecorder
namespace ThermalSim.Domain
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class SimConnectVariableAttribute : Attribute
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public SIMCONNECT_DATATYPE Type { get; set; }
        public SetType SetType { get; set; }
        public string? SetByEvent { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string? Default { get; set; }
    }

    public enum SetType
    {
        Default,
        Event,
        None,
    }
}
