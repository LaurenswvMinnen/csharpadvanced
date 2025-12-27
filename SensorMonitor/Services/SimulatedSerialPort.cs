using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SensorMonitor.Interfaces;

namespace SensorMonitor.Services
{
    public class SimulatedSerialPort : ISensorSubject
    {
        private readonly List<IMeasurementObserver> _observers = new();
        private readonly string _filePath;

        public SimulatedSerialPort(string filePath)
        {
            _filePath = filePath;
        }

        public void Attach(IMeasurementObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IMeasurementObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(string rawData)
        {
            foreach (var observer in _observers)
            {
                observer.Update(rawData);
            }
        }

        public async Task StartReadingAsync(CancellationToken cancellationToken)
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"Error: Sensor data file not found at {_filePath}");
                return;
            }

            // Infinite loop to simulate continuous data
            while (!cancellationToken.IsCancellationRequested)
            {
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Notify(line);
                        }

                        // Simulate delay between readings
                        await Task.Delay(500, cancellationToken); 
                    }
                }
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
