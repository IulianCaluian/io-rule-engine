using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public class ActionEventData
    {
    }
    public class ExecuteProcedureActionEventData : ActionEventData
    {
        public int ProcedureNumber { get; set; }
    }

}
