using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ioRuleEngine.ConsoleApp
{
    public class BarrierRule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public BarrierAction Action { get; set; }
        public BarrierTrigger Trigger { get; set; }
    }

    public enum BarrierAction
    {
        Open,
        Close
    }

    public class BarrierTrigger
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Interval { get; set; }
        public List<ExternalCommand> ExternalCommands { get; set; }
    }

    public class ExternalCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }
    }

    public class BarrierRuleEngine
    {
        private readonly IList<BarrierRule> _rules;
        private readonly IScheduler _scheduler;

        public BarrierRuleEngine(IList<BarrierRule> rules, IScheduler scheduler)
        {
            _rules = rules;
            _scheduler = scheduler;
        }

        public async Task StartAsync()
        {
            await _scheduler.Start();

            foreach (var rule in _rules)
            {
                if (!rule.IsEnabled) continue;

                var triggerBuilder = TriggerBuilder.Create()
                    .WithIdentity(rule.Name)
                    .StartNow();

                if (rule.Trigger.StartTime.HasValue)
                {
                    triggerBuilder.StartAt(rule.Trigger.StartTime.Value);
                }

                if (rule.Trigger.EndTime.HasValue)
                {
                    triggerBuilder.EndAt(rule.Trigger.EndTime.Value);
                }

                if (rule.Trigger.Interval.HasValue)
                {
                    triggerBuilder.WithSimpleSchedule(x =>
                        x.WithInterval(rule.Trigger.Interval.Value)
                        .RepeatForever());
                }

                if (rule.Trigger.ExternalCommands != null)
                {
                    // Add code to register and handle external commands.
                }

                var trigger = triggerBuilder.Build();

                var jobDataMap = new JobDataMap();
                jobDataMap["Action"] = rule.Action;

                var jobDetail = JobBuilder.Create<BarrierJob>()
                    .WithIdentity($"{rule.Name}Job")
                    .UsingJobData(jobDataMap)
                    .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);
            }
        }

        public async Task StopAsync()
        {
            await _scheduler.Shutdown();
        }
    }



}
