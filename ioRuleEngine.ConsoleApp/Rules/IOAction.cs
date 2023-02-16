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
        IOActionType ActionType { get; set; }
    }
}
