using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Interfaces
{
    public class ExternalCommandsGenerator
    {
        public event EventHandler<EventArgs>? CommandReceived;

    }
}
