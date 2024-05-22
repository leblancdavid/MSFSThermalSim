using Microsoft.Extensions.Hosting.WindowsServices;
using System.Net.WebSockets;
using ThermalSim.Api.Services;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Notifications;
using ThermalSim.Domain.Thermals;
using ThermalSim.Domain.Towing;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseWindowsService(); 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISimConnection, SimConnection>();
builder.Services.AddSingleton<IThermalSimulator, ThermalSimulator>();
builder.Services.AddSingleton<ITaxiingService, TaxiingService>();
builder.Services.AddTransient<IThermalGenerator, FixedPositionThermalGenerator>();

builder.Services.AddSingleton<IEventNotifier<WebSocket>, WebSocketService>();

//builder.Services.AddHostedService<ThermalSimulatorBackgroundService>();

builder.Services.AddCors(p => p.AddPolicy("thermalsim", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
});

app.UseHttpsRedirection();

app.UseCors("thermalsim");

app.UseAuthorization();

app.MapControllers();

app.Run("http://localhost:17188");
//app.Run();
