namespace ThermalSim.Domain.Connection
{
    public enum SimDataRequests
    {
        AIRCRAFT_POSITION,
        ENUM_INPUTS,
        INPUT_EVENT
    }

    public enum SimDataEventTypes
    {
        AircraftPosition,
        ThermalVelocityUpdate,
        TurbulenceEffect,
        TowingSpeedUpdate
    }
}
