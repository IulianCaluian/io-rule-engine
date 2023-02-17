using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public enum TriggerSourceEnum
    {
        TimeZone,

        Input,
        Output,

        TriggerVariable,
        Procedure, 

        ExternalCommand

    }

    public enum TriggerEventDataEnum
    {
        SourceActivated,
        SourceDezactivated,

        InputStateChanged,

        Procedure,

        ExternalCommand

    }

    public class IORuleTrigger
    {
        public TriggerSourceEnum TriggerSource { get; set; }
        public TriggerEventDataEnum TriggerEventData { get; set; }
    }
}
