using SensorMonitor.Models;

namespace SensorMonitor.Patterns.State
{
    public class SensorContext
    {
        public string SensorName { get; }
        public SensorState CurrentState { get; set; }

        public SensorContext(string sensorName)
        {
            SensorName = sensorName;
            CurrentState = new NormalState(); // Initial state
        }

        public void Update(SensorMeasurement measurement)
        {
            CurrentState.Handle(this, measurement);
        }
    }
}
