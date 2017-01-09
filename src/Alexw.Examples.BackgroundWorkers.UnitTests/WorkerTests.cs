using System;
using System.Threading;
using NUnit.Framework;

namespace Alexw.Examples.BackgroundWorkers.UnitTests
{
    [TestFixture]
    public class WorkerTests
    {
        [Test]
        public void WorkerRunningInBackgroundThread_TerminatesProperly()
        {
            var worker = new Worker();
            var thread = new Thread(() =>
            {
                worker.Start();
            });
            thread.Start();

            var fiveSecondsFromNow = DateTime.UtcNow.AddSeconds(5);
            DoUntil(() => worker.LastSignalTime > fiveSecondsFromNow, (i, t) => TimeSpan.Zero, (i, t) => i < 100);
            Assert.True(worker.LastSignalTime > fiveSecondsFromNow);
        }

        /// <summary>
        /// Loop until a condition is met and your function returns true
        /// </summary>
        /// <param name="action">The action to perform. Return true to exit the loop.</param>
        /// <param name="delay">Return a timespan for the delay between loops</param>
        /// <param name="whentoStop">Return true when the loop should stop when catching exceptions or looping</param>
        public static void DoUntil(Func<bool> action, Func<int, TimeSpan, TimeSpan> delay, Func<int, TimeSpan, bool> whentoStop)
        {
            var startedUtc = DateTime.UtcNow;
            var amountOfRetriesAllowed = 0;
            while (true)
            {
                try
                {
                    if (action()) return;
                }
                catch
                {
                    if (whentoStop(amountOfRetriesAllowed, DateTime.UtcNow.Subtract(startedUtc))) throw;
                }
                finally
                {
                    amountOfRetriesAllowed++;
                }

                var calculatedDelay = delay(amountOfRetriesAllowed, DateTime.UtcNow.Subtract(startedUtc));
                if (calculatedDelay.TotalMilliseconds < 0)
                {
                    calculatedDelay = TimeSpan.Zero;
                }
                Thread.Sleep(calculatedDelay);
            }
        }
    }
}