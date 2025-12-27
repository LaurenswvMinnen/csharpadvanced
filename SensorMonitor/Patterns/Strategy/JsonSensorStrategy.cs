using System;
using System.Text.Json;
using SensorMonitor.Interfaces;
using SensorMonitor.Models;

namespace SensorMonitor.Patterns.Strategy
{
    public class JsonSensorStrategy : ISensorDataStrategy
    {
        public bool CanHandle(string rawData)
        {
            return rawData.Trim().StartsWith("{");
        }

        public SensorMeasurement Parse(string rawData)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                // map to a DTO matching the JSON structure.
                var data = JsonSerializer.Deserialize<JsonData>(rawData, options);

                if (data == null) throw new Exception("Failed to deserialize JSON");

                // Samsung specific logic 1920 -> 19.20 
                double tempC = double.Parse(data.Temp) / 100.0;

                return new SensorMeasurement
                {
                    SensorName = data.Serial,
                    SensorType = data.Manu,
                    Temperature = tempC,
                    Unit = "Celsius",
                    BatteryLevel = double.TryParse(data.Bat, out double bat) ? bat : null,
                    RawData = rawData,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON Parse Error: {ex.Message}");
                throw;
            }
        }

        private class JsonData
        {
            public string Serial { get; set; }
            public string Bat { get; set; }
            public string Temp { get; set; }
            public string Manu { get; set; }
        }
    }
}
