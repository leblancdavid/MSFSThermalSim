using Microsoft.Extensions.Logging;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Thermals;

namespace ThermalSim
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<ISimConnection, SimConnection>();
            builder.Services.AddSingleton<IThermalSimulator, ThermalSimulator>();
            builder.Services.AddTransient<IThermalGenerator, FixedPositionThermalGenerator>();

            builder.Services.AddSingleton<MainPageViewModel>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
