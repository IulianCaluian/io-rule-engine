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
using ioRulesEngine.ConsoleApp.Utils;
using System.Diagnostics;
using System.Reflection;
using ioRulesEngine.ConsoleApp.Rules.Handlers;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public class RulesEngine
    {
        private readonly List<IORule>? _rules;
        Dictionary<int, IOProcedure> _registeredProcedures;

        // Device
        DeviceEventsHandler? _deviceEventsHandler;
        ExternCmdsEventsHandler? _externCmdEventsHandler;

        // Scheduler
        private IScheduler? _scheduler;
        private Dictionary<Guid, IOProcedure> _timeTriggeredProcedures;

        // VariableTriggeredProcedures:
        private bool[] _triggerVariables = new bool[128];
        private List<Tuple<int,IOProcedure>> _variableTriggeredProcedures;

        public RulesEngine(
            List<IORule>? rules,
             Dictionary<int, IOProcedure>? registeredProcedures,
            IDeviceProcessor? deviceProcessor,
            ExternalCommandsGenerator? externalCommandsGenerator)
        {
            _rules = rules;
            _registeredProcedures = registeredProcedures ?? new Dictionary<int, IOProcedure>();

            if (deviceProcessor != null)
                _deviceEventsHandler = new DeviceEventsHandler(deviceProcessor, ExecuteProcedureDelegateImplementation);

            if (externalCommandsGenerator != null)
                _externCmdEventsHandler = new ExternCmdsEventsHandler(externalCommandsGenerator, ExecuteProcedureDelegateImplementation);

            _timeTriggeredProcedures = new Dictionary<Guid, IOProcedure>();
            _variableTriggeredProcedures = new List<Tuple<int, IOProcedure>>();
        }

        private async Task ExecuteProcedureDelegateImplementation(IOProcedure procedure)
        {
            await ExecuteProcedure(procedure);
        }

        /// <summary>
        /// Start the rule engine by wiring the triggers.
        /// <exception cref="InvalidOperationException">Thrown if some necessary parts are mising for triggers.</exception>
        public async Task StartAsync()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = await schedulerFactory.GetScheduler();

            if (_rules == null)
                throw new InvalidOperationException("No rules are defined for this RulesEngine.");

            foreach (var rule in _rules)
            {
                if (!rule.IsEnabled) continue;

                //TODO Write a quartz job that enbles or disable a rule based on TimetableZon
                //     until then we asume that the rule is enabled and always active.

                WireUpTrigger(rule);
            }

            await _scheduler.Start();
        }

        private async void WireUpTrigger(IORule ioRule)
        {
            switch (ioRule.Trigger.TriggerSource)
            {
                case TriggerSourceEnum.TriggerVariable:
                    {
                        WireUpRuleToATriggerVariaable(ioRule);
                        Debug.WriteLine("here registered a rule");
                    }
                    break;

                case TriggerSourceEnum.Input:
                    {
                        _deviceEventsHandler?.AddInputTriggeredRule(ioRule);
                    }
                    break;

                case TriggerSourceEnum.Output:
                    {
                        _deviceEventsHandler?.AddOutputTriggeredRule(ioRule);
                    }
                    break;

                case TriggerSourceEnum.ExternalCommand:
                    {
                        _externCmdEventsHandler?.AddRule(ioRule);
                    }
                    break;

                case TriggerSourceEnum.TimeEvent:
                    {
                        await SetUpAnEventAtASpecificTime(ioRule);
                    }
                    break;

            }
        }


        public async Task StopAsync()
        {
            if (_scheduler != null)
            {
                await _scheduler.Shutdown();
            }
        }

        public async Task SetTriggerVariable(int triggerVariableNumber, bool activated)
        {
            if (triggerVariableNumber < 1 || triggerVariableNumber > 127)
                return;

            if (activated == true)
            {
                foreach (var pair in _variableTriggeredProcedures)
                    if (pair.Item1 == triggerVariableNumber)
                    {
                        await ExecuteProcedure(pair.Item2);
                    }
            }

            _triggerVariables[triggerVariableNumber] = activated;

        }

        public async Task ExecuteRegisteredProcedure(int number)
        {
            if (_registeredProcedures.ContainsKey(number))
                await ExecuteProcedure(_registeredProcedures[number]);
        }

        private void InjectRulesEngineDependency(IOProcedure procedure)
        {
            // Inject RulesEngine where is needed
            foreach (IOAction action in procedure.Actions)
                if (action is ExecuteRegisteredProcedureAction)
                {
                    ExecuteRegisteredProcedureAction erpa = (ExecuteRegisteredProcedureAction)action;
                    erpa.SetRulesEngine(this);
                }
        }

        private async Task ExecuteProcedure(IOProcedure procedure)
        {
            InjectRulesEngineDependency(procedure);

            await procedure.Execute();
        }



        #region Trigger variable events 
        public void WireUpRuleToATriggerVariaable(IORule ioRule)
        {
            var tvTED = ioRule.Trigger.TriggerEventData;

            if (tvTED == null || tvTED is not TriggerVariableEventData)
                throw new InvalidOperationException("TriggerVariable-event trigger event-data is missing.");

            TriggerVariableEventData tvEvent = (TriggerVariableEventData)tvTED;

            _variableTriggeredProcedures.Add(new Tuple<int, IOProcedure>(tvEvent.TriggerVariableNumber, ioRule.Procedure));
        }
        #endregion

        #region Time triggered rules
        
        private const string TIME_BASED_JOB_TRIGGER = "timeBasedJobTrg";
        private const string TIME_BASED_JOB = "timeBasedJob";
        private const string TimeBasedJobDataProcedure = "jobDataProcedure";

        private async Task SetUpAnEventAtASpecificTime(IORule ioRule)
        {
            // Time is ioRule.Trigger.TriggerSource == TriggerSourceEnum.TimeZone;
            var timeTED = ioRule.Trigger.TriggerEventData;

            if (timeTED == null || timeTED is not TimeEventTriggerEventData)
                throw new InvalidOperationException("Time-event trigger event-data is missing.");

            TimeEventTriggerEventData timeEvent = (TimeEventTriggerEventData)timeTED;
            int hourToStart = timeEvent.Hour;
            int minuteToStart = timeEvent.Minute;

            Guid guid = Guid.NewGuid();

            Console.WriteLine($"Time trigger registered with guid: {guid}");

            // Define the trigger to fire at 6pm
            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity($"{TIME_BASED_JOB_TRIGGER}{guid}", guid.ToString())
                .WithDailyTimeIntervalSchedule(s =>
                    s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hourToStart, minuteToStart)))
                .Build();

            InjectRulesEngineDependency(ioRule.Procedure);

            // Define the job to execute when the trigger fires
            IJobDetail job1 = JobBuilder.Create<TimeBasedJob>()
                .WithIdentity($"{TIME_BASED_JOB}{guid}", guid.ToString())
                .UsingJobData(new JobDataMap(new Dictionary<string, IOProcedure>()
                {
                    {TimeBasedJobDataProcedure , ioRule.Procedure  }
                }))
                .Build();

            // Schedule the job to run at the specified time
            if (_scheduler != null)
            {
                await _scheduler.ScheduleJob(job1, trigger1);
            }
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
