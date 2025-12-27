using System;
using System.Xml.Linq;
using System.Globalization;
using SensorMonitor.Interfaces;
using SensorMonitor.Models;

namespace SensorMonitor.Patterns.Strategy
{
    public class XmlSensorStrategy : ISensorDataStrategy
    {
        public bool CanHandle(string rawData)
        {
            return rawData.Trim().StartsWith("<");
        }

        public SensorMeasurement Parse(string rawData)
        {
            try
            {
                var doc = XDocument.Parse(rawData);
                var root = doc.Element("msg");

                string tempStr = root?.Element("temp")?.Value ?? "0";
                
                // Handle comma decimal separator
                if (double.TryParse(tempStr.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double tempK))
                {
                    // Philips specific logic: Temp is in Kelvin
                    double tempC = tempK - 273.15;

                    return new SensorMeasurement
                    {
                        SensorName = root?.Element("serial")?.Value ?? "Unknown",
                        SensorType = root?.Element("manu")?.Value ?? "Philips",
                        Temperature = Math.Round(tempC, 2),
                        Unit = "Celsius",
                        BatteryLevel = double.TryParse(root?.Element("bat")?.Value, out double bat) ? bat : null,
                        RawData = rawData,
                        Timestamp = DateTime.UtcNow
                    };
                }
                throw new Exception("Invalid temperature format in XML");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"XML Parse Error: {ex.Message}");
                throw;
            }
        }
    }
}
