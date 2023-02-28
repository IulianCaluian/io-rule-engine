using ioRulesEngine.ConsoleApp.Devices;
using ioRulesEngine.ConsoleApp.Interfaces;
using ioRulesEngine.ConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules.Handlers
{
    public class ExternCmdsEventsHandler
    {
        private ExecuteProcedureDelegate _executeProcedure;

        private readonly ExternalCommandsGenerator _externalCommandsGenerator;
        private List<IORule> _externalCommandsTriggeredRules;


        public ExternCmdsEventsHandler(ExternalCommandsGenerator externalCommandsGenerator, ExecuteProcedureDelegate executeProcedure)
        {
            _externalCommandsGenerator = externalCommandsGenerator;
            _externalCommandsGenerator.CommandReceived += new EventHandler<EventArgs>(CommandGeneratorCommandReceived);

            _executeProcedure = executeProcedure;

            _externalCommandsTriggeredRules = new List<IORule>();
        }


        private void CommandGeneratorCommandReceived(object? sender, EventArgs e)
        {
            foreach (var rule in _externalCommandsTriggeredRules)
            {
                // Test if the rule is activated for specific input. rule.Trigger.Source
            }
        }

        internal void AddRule(IORule ioRule)
        {
            _externalCommandsTriggeredRules.Add(ioRule);
        }

    }
}
