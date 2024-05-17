using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Notifications
{
    public class AircraftThermalPositionEvent
    {
        public ThermalPositionState ThermalState { get; set; }
        public double CurrentLift { get; set; }
        public double AircraftHeading { get; set; }
        public double RelativeNearestThermal { get; set; }
        public double NearestThermalDistance { get; set; }
        public double WindHeading { get; set; }
        public double WindSpeed { get; set; }


    }
}
