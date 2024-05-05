namespace ThermalSim.Domain.Connection
{
    public enum SimDataRequests
    {
        AIRCRAFT_POSITION
    }

    public enum SimDataEventTypes
    {
        AircraftPosition,
        ThermalVelocityUpdate,
        TurbulenceEffect,
        TowingSpeedUpdate
    }
}
