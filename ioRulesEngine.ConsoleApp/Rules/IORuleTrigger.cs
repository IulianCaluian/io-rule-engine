﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{
    public enum TriggerSourceEnum
    {
        TriggerVariable,

        TimeEvent,

        Input,
        Output,

        Procedure, 

        ExternalCommand
    }

    public class IORuleTrigger
    {
        public TriggerSourceEnum TriggerSource { get; set; }
        public TriggerEventData? TriggerEventData { get; set; } = null;
    }
}
