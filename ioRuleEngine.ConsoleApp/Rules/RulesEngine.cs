using ioRulesEngine.ConsoleApp.Devices;
using ioRulesEngine.ConsoleApp.Interfaces;
using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public class RulesEngine
    {
        private readonly List<IORule> _rules;

        // Device
        private readonly DeviceProcessor _deviceProcessor;
        private bool _inputEventsWired = false;
        private List<IORule> _inputTriggeredRules;
        private bool _outputEventsWired = false;
        private List<IORule> _outputTriggeredRules;

        // External commands
        private ExternalCommandsGenerator _externalCommandsGenerator;
        private bool _externalCommandsEventsWired = false;
        private List<IORule> _externalCommandsTriggeredRules;

        // Scheduler
        private IScheduler _scheduler;
        private Dictionary<Guid, IOProcedure> _timeTriggeredProcedures;

        public RulesEngine(List<IORule> rules, DeviceProcessor deviceProcessor)
        {
            _rules = rules;
            _deviceProcessor = deviceProcessor;

            _inputTriggeredRules = new List<IORule>();
            _outputTriggeredRules = new List<IORule>();
        }

        public async Task StartAsync()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = await schedulerFactory.GetScheduler();

            foreach (var rule in _rules)
            {
                if (!rule.IsEnabled) continue;

                //TODO Write a quartz job that enbles or disable a rule based on TimetableZon
                //     until then we asume that the rule is enabled and always active.

                WireUpTrigger(rule);
            }

            await _scheduler.Start();
        }

        private void WireUpTrigger(IORule ioRule)
        {
            switch (ioRule.Trigger.TriggerSource)
            {
                case TriggerSourceEnum.Input:
                    WireUpEventsForDeviceInputsEvents();
                    _inputTriggeredRules.Add(ioRule);
                    break;

                case TriggerSourceEnum.Output:
                    WireUpEventsForDeviceOutputEvents();
                    _outputTriggeredRules.Add(ioRule);
                    break;

                case TriggerSourceEnum.ExternalCommand:
                    WireUpEventsForExternalCommandsEvents();
                    _externalCommandsTriggeredRules.Add(ioRule);
                    break;

                case TriggerSourceEnum.TimeZone:
                    {
                        SetUpAnEventAtASpecificTime(ioRule);
                    }
                    break;

                    // TimeZone,
                    // TriggerVariable,
                    // Procedure, 

        
            }
        }

        public async Task StopAsync()
        {
            await _scheduler.Shutdown();
        }


        #region Device events
        private void WireUpEventsForDeviceInputsEvents()
        {
            if (_inputEventsWired == false)
            {
                _deviceProcessor.InputChanged += new EventHandler<EventArgs>(InputChangedEventGenerated);
                _inputEventsWired = true;
            }
        }

        private void InputChangedEventGenerated(object? sender, EventArgs e)
        {
            foreach(var rule in _inputTriggeredRules)
            {
                // Test if the rule is activated for specific input. rule.Trigger.Source
            }
        }

        private void WireUpEventsForDeviceOutputEvents()
        {
            if (_outputEventsWired == false)
            {
                _deviceProcessor.OutputChanged += new EventHandler<EventArgs>(OutputChangedEventGenerated);
                _outputEventsWired = true;
            }
        }

        private void OutputChangedEventGenerated(object? sender, EventArgs e)
        {
            foreach (var rule in _outputTriggeredRules)
            {
                // Test if the rule is activated for specific input. rule.Trigger.Source
            }
        }
        #endregion

        #region External commands
        private void WireUpEventsForExternalCommandsEvents()
        {
            if (_externalCommandsEventsWired == false)
            {
                _externalCommandsGenerator.CommandReceived += new EventHandler<EventArgs>(CommandGeneratorCommandReceived);
                _externalCommandsEventsWired = true;
            }
        }

        private void CommandGeneratorCommandReceived(object? sender, EventArgs e)
        {
            foreach (var rule in _externalCommandsTriggeredRules)
            {
                // Test if the rule is activated for specific input. rule.Trigger.Source
            }
        }
        #endregion

        #region Time triggered rules
        
        private const string TIME_BASED_JOB_TRIGGER = "timeBasedJobTrg";
        private const string TIME_BASED_JOB = "timeBasedJob";
        private const string TimeBasedJobDataProcedure = "jobDataProcedure";

        private void SetUpAnEventAtASpecificTime(IORule ioRule)
        {
            // Time is ioRule.Trigger.TriggerSource == TriggerSourceEnum.TimeZone;
            int hourToStart = 14;
            int minuteToStart = 0;

            Guid guid = Guid.NewGuid();

            // Define the trigger to fire at 6pm
            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity( $"{TIME_BASED_JOB_TRIGGER}{guid}", guid.ToString())
                .WithDailyTimeIntervalSchedule(s =>
                    s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hourToStart, minuteToStart)))
                .Build();

            // Define the job to execute when the trigger fires
            _timeTriggeredProcedures.Add(guid, ioRule.Procedure);

            IJobDetail job1 = JobBuilder.Create<TimeBasedJob>()
                .WithIdentity($"{TIME_BASED_JOB}{guid}", guid.ToString())
                .UsingJobData(new JobDataMap(new Dictionary<string, IOProcedure>()
                {
                    {TimeBasedJobDataProcedure , ioRule.Procedure  }
                }))
                .Build();

            // Schedule the job to run at the specified time
            _scheduler.ScheduleJob(job1, trigger1).Wait();
        }

        public class TimeBasedJob : IJob
        {

            public async Task Execute(IJobExecutionContext context)
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                IOProcedure procedure =  (IOProcedure)dataMap[TimeBasedJobDataProcedure];

                await procedure.Execute();
            }
        }

        #endregion
    }
}
