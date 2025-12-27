using System.Collections.Generic;
using SensorMonitor.Interfaces;
using SensorMonitor.Patterns.Strategy;

namespace SensorMonitor.Patterns.Singleton
{
    public class SensorStrategyFactory
    {
        private static SensorStrategyFactory _instance;
        private static readonly object _lock = new object();

        private readonly List<ISensorDataStrategy> _strategies;

        private SensorStrategyFactory()
        {
            _strategies = new List<ISensorDataStrategy>
            {
                new JsonSensorStrategy(),
                new XmlSensorStrategy(),
                new KeyValueSensorStrategy()
            };
        }

        public static SensorStrategyFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SensorStrategyFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        public ISensorDataStrategy GetStrategy(string rawData)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.CanHandle(rawData))
                {
                    return strategy;
                }
            }
            // Default or fallback
            return _strategies.Find(s => s is KeyValueSensorStrategy); 
        }
    }
}
