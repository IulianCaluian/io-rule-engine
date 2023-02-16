using ioRulesEngine.ConsoleApp.Devices;
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
        private readonly DeviceProcessor _deviceProcessor;

        private bool _inputEventsWired = false;
        private List<IORule> _inputTriggeredRules;

        private bool _outputEventsWired = false;
        private List<IORule> _outputTriggeredRules;

        public RulesEngine(List<IORule> rules, DeviceProcessor deviceProcessor)
        {
            _rules = rules;
            _deviceProcessor = deviceProcessor;
            
        }

        public async Task StartAsync()
        {
            foreach (var rule in _rules)
            {
                if (!rule.IsEnabled) continue;

                //TODO Write a quartz job that enbles or disable a rule based on TimetableZon
                //     until then we asume that the rule is enabled and always active.

                WireUpTrigger(rule);
            }
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
            }
        }



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
    }
}
