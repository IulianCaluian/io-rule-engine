using ioRulesEngine.ConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Rules
{

    public class IORule
    {
        public bool IsEnabled { get; set; }
        public TimetableZone TimetableZone { get; set; }

        public IORuleTrigger Trigger { get; private set; }
        public IOProcedure Procedure { get; private set; }

        public IORule(IORuleTrigger trigger, IOProcedure procedure)
        {
            IsEnabled = true;
            TimetableZone = TimetableZone.Default();

            Trigger = trigger;
            Procedure = procedure;
          

        }
    }
}
