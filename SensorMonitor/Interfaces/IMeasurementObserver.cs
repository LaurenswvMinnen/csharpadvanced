namespace SensorMonitor.Interfaces
{
    public interface IMeasurementObserver
    {
        void Update(string rawData);
    }
}
