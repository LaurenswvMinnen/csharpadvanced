namespace SensorMonitor.Models
{
    public class SensorMeasurement
    {
        public string SensorName { get; set; } = string.Empty;
        public string SensorType { get; set; } = string.Empty; 
        public double Temperature { get; set; }
        public string Unit { get; set; } = "Celsius";
        public double? BatteryLevel { get; set; }
        public double? Humidity { get; set; }
        public DateTime Timestamp { get; set; }
        public string RawData { get; set; } = string.Empty;
    }
}
