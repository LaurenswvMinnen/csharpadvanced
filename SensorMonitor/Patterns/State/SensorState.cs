using System;
using SensorMonitor.Models;

namespace SensorMonitor.Patterns.State
{
    public abstract class SensorState
    {
        public abstract void Handle(SensorContext context, SensorMeasurement measurement);
    }

    public class NormalState : SensorState
    {
        public override void Handle(SensorContext context, SensorMeasurement measurement)
        {
            // Transition logic          
            if (measurement.Temperature > 50)
            {
                context.CurrentState = new WarningState();
                Console.WriteLine($"[ALERT] Sensor {context.SensorName} measured {measurement.Temperature}C - Switching to WARNING.");
            }
        }
    }

    public class WarningState : SensorState
    {
        public override void Handle(SensorContext context, SensorMeasurement measurement)
        {
            if (measurement.Temperature > 80)
            {
                context.CurrentState = new ErrorState();
                Console.WriteLine($"[ALARM] Sensor {context.SensorName} measured {measurement.Temperature}C - Switching to ERROR.");
            }
            else if (measurement.Temperature < 45)
            {
                context.CurrentState = new NormalState();
                Console.WriteLine($"[INFO] Sensor {context.SensorName} normalized - Switching to NORMAL.");
            }
        }
    }

    public class ErrorState : SensorState
    {
        public override void Handle(SensorContext context, SensorMeasurement measurement)
        {
            if (measurement.Temperature < 60)
            {
                context.CurrentState = new WarningState();
                Console.WriteLine($"[INFO] Sensor {context.SensorName} recovering - Switching to WARNING.");
            }
        }
    }
}
