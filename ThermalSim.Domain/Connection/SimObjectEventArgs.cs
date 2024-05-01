namespace ThermalSim.Domain.Connection
{
    public class SimObjectEventArgs : EventArgs
    {
        public SimObjectEventArgs(SimObject simObject)
        {
            SimObject = simObject;
        }
        public SimObject SimObject { get; set; }
    }

}
