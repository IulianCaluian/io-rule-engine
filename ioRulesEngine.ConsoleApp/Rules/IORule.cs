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

        public IORuleTrigger Trigger { get; set; }
        public IOProcedure Procedure { get; set; }

        public IORule()
        {
            IsEnabled = true;
            TimetableZone = TimetableZone.Default();
          

        }
    }
}
