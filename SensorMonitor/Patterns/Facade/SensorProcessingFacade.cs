using System;
using System;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using SensorMonitor.Hubs;
using SensorMonitor.Interfaces;
using SensorMonitor.Models;
using SensorMonitor.Patterns.Singleton;
using SensorMonitor.Patterns.State;

namespace SensorMonitor.Patterns.Facade
{
    // Facade implementing Observer to receive updates
    public class SensorProcessingFacade : IMeasurementObserver
    {
        private readonly ISensorSubject _subject;
        private readonly IHubContext<SensorHub> _hubContext;
        // Thread-safe dictionary for sensor states
        private readonly ConcurrentDictionary<string, SensorContext> _sensorContexts = new();

        public SensorProcessingFacade(ISensorSubject subject, IHubContext<SensorHub> hubContext)
        {
            _subject = subject;
            _hubContext = hubContext;
            _subject.Attach(this);
        }

        public void Update(string rawData)
        {
            ProcessData(rawData);
        }

        private void ProcessData(string rawData)
        {
            try
            {
                
                string cleanData = CleanData(rawData);
                if (string.IsNullOrEmpty(cleanData)) return;

                //Get Strategy via Singleton
                var strategy = SensorStrategyFactory.Instance.GetStrategy(cleanData);

                // Parse Data
                SensorMeasurement measurement = strategy.Parse(cleanData);

                // Apply State Pattern logic
                var context = _sensorContexts.GetOrAdd(measurement.SensorName, name => new SensorContext(name));
                context.Update(measurement);
                


                // Broadcast via SignalR
                _hubContext.Clients.All.SendAsync("ReceiveMeasurement", measurement);
                
                Console.WriteLine($"Processed: {measurement.SensorName} ({measurement.SensorType}) - {measurement.Temperature} {measurement.Unit} - State: {context.CurrentState.GetType().Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing data: {ex.Message}");
            }
        }

        private string CleanData(string rawData)
        {
            // Matches "123: content..." and captures "content..."
            var match = Regex.Match(rawData, @"^\d+:\s*(.*)$");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return rawData;
        }
    }
}
