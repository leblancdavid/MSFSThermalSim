
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Api.Services
{
    public class ThermalSimulatorBackgroundService : IHostedService, IDisposable
    {
        private readonly IThermalSimulator thermalSimulator;
        private readonly ILogger<ThermalSimulatorBackgroundService> logger;

        public ThermalSimulatorBackgroundService(IThermalSimulator thermalSimulator,
            ILogger<ThermalSimulatorBackgroundService> logger)
        {
            this.thermalSimulator = thermalSimulator;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                thermalSimulator.Start();
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                thermalSimulator.Stop();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }


        public void Dispose()
        {
        }
    }
}
