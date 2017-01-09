using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Alexw.Examples.BackgroundWorkers.UnitTests
{
    public class Worker
    {
        private Timer _timer;

        public DateTime LastSignalTime { get; private set; }

        public void Start()
        {
            _timer = new Timer
            {
                Interval = TimeSpan.FromSeconds(1).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            Thread.Sleep(-1);
        }

        public void Stop()
        {
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Stop();
        }

        private void OnTimerElapsed(Object source, ElapsedEventArgs e)
        {
            LastSignalTime = e.SignalTime;
            Console.WriteLine(e.SignalTime + " Timer Elapsed Event");
        }
    }
}
