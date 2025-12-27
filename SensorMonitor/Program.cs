using SensorMonitor.Hubs;
using SensorMonitor.Interfaces;
using SensorMonitor.Patterns.Facade;
using SensorMonitor.Services;
using SensorMonitor.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

string sensorDataPath = Path.Combine(Directory.GetCurrentDirectory(), "sensor_data.txt");
builder.Services.AddSingleton<SimulatedSerialPort>(sp => new SimulatedSerialPort(sensorDataPath));
builder.Services.AddSingleton<ISensorSubject>(sp => sp.GetRequiredService<SimulatedSerialPort>());

builder.Services.AddSingleton<SensorProcessingFacade>();

builder.Services.AddHostedService<SensorWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();
app.MapHub<SensorHub>("/sensorHub");

app.Run();
