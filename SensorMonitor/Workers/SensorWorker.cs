using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using SensorMonitor.Services;
using SensorMonitor.Patterns.Facade;

namespace SensorMonitor.Workers
{
    public class SensorWorker : BackgroundService
    {
        private readonly SimulatedSerialPort _serialPort;
        private readonly SensorProcessingFacade _facade;

        // The serialPort is used to start the loop.
        public SensorWorker(SimulatedSerialPort serialPort, SensorProcessingFacade facade)
        {
            _serialPort = serialPort;
            _facade = facade; 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _serialPort.StartReadingAsync(stoppingToken);
        }
    }
}
