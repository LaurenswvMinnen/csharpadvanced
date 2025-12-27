using SensorMonitor.Models;

namespace SensorMonitor.Interfaces
{
    public interface ISensorDataStrategy
    {
        bool CanHandle(string rawData);
        SensorMeasurement Parse(string rawData);
    }
}
