using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public enum IOActionType
    {
        GenericAction,

        ControllBarrierCommand,
        
        TimeDelay,
        TriggerVariableSet,
        TriggerActivation,

        ExecuteRegisteredProcedure
    }


    public class IOAction
    {
        public IOActionType ActionType { get; set; }

        public bool Executed { get; protected set; } = false;

        public virtual Task Execute()
        {
            Executed = true;

            Console.WriteLine($"Executed action of type {ActionType}.");

            return Task.CompletedTask;
        }
    }

    public class ExecuteRegisteredProcedureAction : IOAction
    {
        public int _procedureNumber; 

        private RulesEngine? _rulesEngine;

        public ExecuteRegisteredProcedureAction(int procedureNumber)
        {
            ActionType = IOActionType.ExecuteRegisteredProcedure;
            _procedureNumber = procedureNumber; 
        }

        public override async Task Execute()
        {
            if (_rulesEngine != null)
            {
                await _rulesEngine.ExecuteRegisteredProcedure(_procedureNumber);
            }

            Executed = true;
        }

        public void SetRulesEngine(RulesEngine rulesEngine)
        {
            _rulesEngine = rulesEngine;
        }
    }

}
