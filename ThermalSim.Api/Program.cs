using ThermalSim.Api.Services;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Thermals;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISimConnection, SimConnection>();
builder.Services.AddSingleton<IThermalSimulator, ThermalSimulator>();
builder.Services.AddTransient<IThermalGenerator, FixedPositionThermalGenerator>();

builder.Services.AddHostedService<ThermalSimulatorBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
