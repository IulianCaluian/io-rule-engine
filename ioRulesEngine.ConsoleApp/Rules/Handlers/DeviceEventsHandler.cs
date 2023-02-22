using ioRulesEngine.ConsoleApp.Devices;
using ioRulesEngine.ConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules.Handlers
{
    

    public class DeviceEventsHandler
    {
        private ExecuteProcedureDelegate _executeProcedure;

        private readonly IDeviceProcessor _deviceProcessor;
        private List<IORule> _inputTriggeredRules;
        private List<IORule> _outputTriggeredRules;
       

        public DeviceEventsHandler(IDeviceProcessor deviceProcessor, ExecuteProcedureDelegate executeProcedure)
        {
            _deviceProcessor = deviceProcessor;
            _deviceProcessor.InputChanged += new AsyncEventHandler<EventArgs>(InputChangedEventGenerated);
            _deviceProcessor.OutputChanged += new AsyncEventHandler<EventArgs>(OutputChangedEventGenerated);

            _executeProcedure = executeProcedure;

            _inputTriggeredRules = new List<IORule>();
            _outputTriggeredRules = new List<IORule>();
        }

        #region Device events


        private async Task InputChangedEventGenerated(object? sender, EventArgs e)
        {
            foreach (var rule in _inputTriggeredRules)
            {
                await _executeProcedure(rule.Procedure);
                // OLD await ExecuteProcedure(rule.Procedure);
            }
        }


        private async Task OutputChangedEventGenerated(object? sender, EventArgs e)
        {
            foreach (var rule in _outputTriggeredRules)
            {
                await _executeProcedure(rule.Procedure);
                // OLD await ExecuteProcedure(rule.Procedure);
            }
        }

        internal void AddInputTriggeredRule(IORule ioRule)
        {
            _inputTriggeredRules.Add(ioRule);
        }

        internal void AddOutputTriggeredRule(IORule ioRule)
        {
            _outputTriggeredRules.Add(ioRule);
        }

        #endregion
    }
}
