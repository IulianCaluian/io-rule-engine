using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRuleEngine.ConsoleApp
{
    /// <summary>
    /*
     * // Example usage
        var scheduler = new BarrierScheduler();
        await scheduler.Start();

        // Wait for the scheduler to run for some time
        await Task.Delay(TimeSpan.FromHours(10));

        await scheduler.Stop();
     */
    /// </summary>
    public class BarrierScheduler
    {
        private IScheduler _scheduler;

        public async Task Start()
        {
            // Create a new scheduler
            var factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();

            // Define the job to execute
            var job = JobBuilder.Create<BarrierJob>()
                .WithIdentity("BarrierJob", "BarrierGroup")
                .Build();

            // Define the trigger to run the job at 9am and 6pm every day
            var trigger = TriggerBuilder.Create()
                .WithIdentity("BarrierTrigger", "BarrierGroup")
                .WithDailyTimeIntervalSchedule(x => x
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(9, 0))
                    .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(18, 0))
                )
                .Build();

            // Schedule the job with the trigger
            await _scheduler.ScheduleJob(job, trigger);

            // Start the scheduler
            await _scheduler.Start();
        }

        public async Task Stop()
        {
            // Shut down the scheduler
            await _scheduler.Shutdown();
        }
    }
}
