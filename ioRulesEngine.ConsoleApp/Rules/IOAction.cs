using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public enum IOActionType
    {
        ControllBarrierCommand,
        ProcedureCommand,
        TimeDelay,
        TriggerVariableSet,
        TriggerActivation
    }

    public class IOAction
    {
        public IOActionType ActionType { get; set; }
        public bool Executed { get; private set; } = false;

        public  Task Execute()
        {
            Executed = true;

            Console.WriteLine($"Executed action of type {ActionType}.");

            return Task.CompletedTask;
        }
    }
}
