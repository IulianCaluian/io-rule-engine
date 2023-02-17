using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public class TriggerEventData
    {

    }

    public class TimeEventTriggerEventData : TriggerEventData
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }

    public class ActivationEventData : TriggerEventData
    {
        public bool Activated { get; set; }
    }

    public class ExternCmdEventData : TriggerEventData
    {

    }

    public class RuleActivatedEventData : TriggerEventData
    {
        public int RuleNumber { get; set; }
    }

    public class TriggerVariableEventData : TriggerEventData
    {
        public int TriggerVariableNumber { get; set; }
    }

    public class ProcedureEventData : TriggerEventData
    {
        public int ProcedureNumber { get; set; }
    }
}
