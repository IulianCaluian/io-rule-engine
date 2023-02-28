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
        TimeEventsHandler _timeEventsHandler;



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

             _timeEventsHandler = new TimeEventsHandler(this);

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

            await _timeEventsHandler.InitializeTimeEventsHandler();

            if (_rules == null)
                throw new InvalidOperationException("No rules are defined for this RulesEngine.");

            foreach (var rule in _rules)
            {
                if (!rule.IsEnabled) continue;

                //TODO Write a quartz job that enbles or disable a rule based on TimetableZon
                //     until then we asume that the rule is enabled and always active.

                await WireUpTrigger(rule);
            }

            await _timeEventsHandler.StartTimeEventsHandler();
        }

        private async Task WireUpTrigger(IORule ioRule)
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
                        await _timeEventsHandler.SetUpAnEventAtASpecificTime(ioRule);
                    }
                    break;

            }
        }


        public async Task StopAsync()
        {
            await _timeEventsHandler.ShutdownTimeEventsHandler();
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



        private async Task ExecuteProcedure(IOProcedure procedure)
        {
            InjectRulesEngineDependency(procedure);

            await procedure.Execute();
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

    }
}
