using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using SensorMonitor.Models;

namespace SensorMonitor.Hubs
{
    public class SensorHub : Hub
    {
        public async Task SendMeasurement(SensorMeasurement measurement)
        {
            await Clients.All.SendAsync("ReceiveMeasurement", measurement);
        }
    }
}
