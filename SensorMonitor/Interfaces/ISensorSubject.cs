namespace SensorMonitor.Interfaces
{
    public interface ISensorSubject
    {
        void Attach(IMeasurementObserver observer);
        void Detach(IMeasurementObserver observer);
        void Notify(string rawData);
    }
}
