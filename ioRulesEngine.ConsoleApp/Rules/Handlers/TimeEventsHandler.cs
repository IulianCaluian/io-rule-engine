using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules.Handlers
{
    public class TimeEventsHandler
    {
        private const string TIME_BASED_JOB_TRIGGER = "timeBasedJobTrg";
        private const string TIME_BASED_JOB = "timeBasedJob";
        private const string TimeBasedJobDataProcedure = "jobDataProcedure";


        private RulesEngine _ruleEngineReference;

        // Scheduler
        private IScheduler? _scheduler;
        private Dictionary<Guid, IOProcedure> _timeTriggeredProcedures;

        public TimeEventsHandler(RulesEngine ruleEngineReference)
        {
            _timeTriggeredProcedures = new Dictionary<Guid, IOProcedure>();
            _ruleEngineReference = ruleEngineReference;
        }

        public async Task InitializeTimeEventsHandler()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = await schedulerFactory.GetScheduler();
        }
        public async Task StartTimeEventsHandler()
        {
            if (_scheduler != null)
            {
                await _scheduler.Start();
            }
        }

        public async Task ShutdownTimeEventsHandler()
        {
            if (_scheduler != null)
            {
                await _scheduler.Shutdown();
            }
        }




        internal async Task SetUpAnEventAtASpecificTime(IORule ioRule)
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


        private void InjectRulesEngineDependency(IOProcedure procedure)
        {
            // Inject RulesEngine where is needed
            foreach (IOAction action in procedure.Actions)
                if (action is ExecuteRegisteredProcedureAction)
                {
                    ExecuteRegisteredProcedureAction erpa = (ExecuteRegisteredProcedureAction)action;
                    erpa.SetRulesEngine(_ruleEngineReference);
                }
        }



        private class TimeBasedJob : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                IOProcedure procedure = (IOProcedure)dataMap[TimeBasedJobDataProcedure];

                await procedure.Execute();
            }
        }

    }


}
