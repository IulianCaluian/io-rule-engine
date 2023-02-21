using ioRulesEngine.ConsoleApp.Devices;
using ioRulesEngine.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public class RulesEngineBuilder
    {
        List<IORule>? _rules = null;
        Dictionary<int, IOProcedure>? _registeredProcedures;
        IDeviceProcessor? _deviceProcessor = null;
        ExternalCommandsGenerator? _externalCommandsGenerator = null;

        public RulesEngineBuilder Rules(List<IORule> rules)
        {
            _rules = rules;
            return this;
        }

        public RulesEngineBuilder RegisteredProcedures(Dictionary<int, IOProcedure> registeredProcedures)
        {
            _registeredProcedures = registeredProcedures;
            return this;
        }

        public RulesEngineBuilder DeviceProcessor(IDeviceProcessor deviceProcessor)
        {
            _deviceProcessor = deviceProcessor;
            return this;
        }

        public RulesEngineBuilder ExternalCommandsGenerator(ExternalCommandsGenerator externalCommandsGenerator)
        {
            _externalCommandsGenerator = externalCommandsGenerator;
            return this;
        }

        /// <summary>
        /// Builds a new instance of the rules engine using the provided rules and other components.
        /// </summary>
        /// <returns>A new instance of the rules engine.</returns>
        /// <exception cref="ArgumentException">Thrown if no rules are provided.</exception>
        public RulesEngine Build()
        {
            if (_rules == null)
                throw new ArgumentException("Rules were not provided for this rules engine.");

            return new RulesEngine(_rules, _registeredProcedures, _deviceProcessor, _externalCommandsGenerator);
        } 
    }
}
