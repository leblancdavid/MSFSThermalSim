namespace ThermalSim.Domain.Connection
{
    public enum SimDataRequests
    {
        AIRCRAFT_POSITION
    }

    public enum SimEvents
    {
        FRAME
    }

    public enum SimDataEventTypes
    {
        Frame,
        AircraftPosition,
        NewThermal,
    }
}
