using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SensorMonitor.Interfaces;
using SensorMonitor.Models;

namespace SensorMonitor.Patterns.Strategy
{
    public class KeyValueSensorStrategy : ISensorDataStrategy
    {
        public bool CanHandle(string rawData)
        {
            // Fallback for anything not JSON/XML
            return !rawData.Trim().StartsWith("{") && !rawData.Trim().StartsWith("<");
        }

        public SensorMeasurement Parse(string rawData)
        {
            var dict = ParseKeyValue(rawData);

            // Check for temp
            if (!dict.ContainsKey("temp")) throw new Exception("Missing temp data");

            double temp = double.Parse(dict["temp"], System.Globalization.CultureInfo.InvariantCulture);
            string manu = dict.ContainsKey("manu") ? dict["manu"] : (dict.ContainsKey("manufac") ? dict["manufac"] : "Unknown");

            // Check for serial
            string serial = "Unknown";
            if (dict.ContainsKey("serial")) serial = dict["serial"];
            else if (dict.ContainsKey("serialnumber")) serial = dict["serialnumber"];

            return new SensorMeasurement
            {
                SensorName = serial,
                SensorType = manu,
                Temperature = temp,
                Unit = "Celsius",
                BatteryLevel = (dict.ContainsKey("bat") && double.TryParse(dict["bat"], out double bat)) ? bat : null,
                RawData = rawData,
                Timestamp = DateTime.UtcNow
            };
        }

        private Dictionary<string, string> ParseKeyValue(string rawData)
        {
            var dict = new Dictionary<string, string>();
            
            // Console.WriteLine($"DEBUG: Processing RawData: {rawData}");

            char[] separators = new[] { ';', ',', '|' };
            var activeSeparator = separators.FirstOrDefault(s => rawData.Contains(s));
            
            if (activeSeparator != default(char))
            {
                // Console.WriteLine($"DEBUG: Detected separator: {activeSeparator}");
                var pairs = rawData.Split(activeSeparator, StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(':');
                    if (parts.Length == 2)
                    {
                        dict[parts[0].Trim().ToLower()] = parts[1].Trim();
                    }
                }
            }
            else
            {
                // Use a whitelist of known keys
                var knownKeys = new[] { 
                    "serial", "serialnumber", "bat", "batlevel", "batterylevel", "batmax", "batmin", 
                    "temp", "hum", "state", "manu", "manufac", "type", "error", "v", "v2", "v3" 
                };
                var pattern = string.Join("|", knownKeys);
                var regex = new Regex($"({pattern}):", RegexOptions.IgnoreCase);
                var matches = regex.Matches(rawData);

                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    string key = match.Groups[1].Value.Trim().ToLower();
                    
                    int valueStartIndex = match.Index + match.Length;
                    int valueEndIndex;

                    if (i < matches.Count - 1)
                    {
                        // The value ends where the next match starts
                        valueEndIndex = matches[i + 1].Index;
                    }
                    else
                    {
                        // The last value ends at the end of the string
                        valueEndIndex = rawData.Length;
                    }

                    int length = valueEndIndex - valueStartIndex;
                    if (length > 0)
                    {
                        string value = rawData.Substring(valueStartIndex, length).Trim();
                        dict[key] = value;
                        // Console.WriteLine($"DEBUG: Found Key={key} Value={value}");
                    }
                }
            }
            
            return dict;
        }
    }
}
