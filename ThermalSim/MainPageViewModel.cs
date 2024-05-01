using ThermalSim.Domain.Thermals;

namespace ThermalSim
{
    public class MainPageViewModel
    {

        private readonly IThermalSimulator thermalSimulator;

        public MainPageViewModel(IThermalSimulator thermalSimulator)
        {
            this.thermalSimulator = thermalSimulator;
        }

        public void Start()
        {
            thermalSimulator.Start();
        }

        public void Stop()
        {
            thermalSimulator.Stop();
        }
    }
}
