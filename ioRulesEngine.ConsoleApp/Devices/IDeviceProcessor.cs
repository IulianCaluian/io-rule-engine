using ioRulesEngine.ConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Devices
{
    public interface IDeviceProcessor
    {
        public event AsyncEventHandler<EventArgs>? InputChanged;
        public event AsyncEventHandler<EventArgs>? OutputChanged;
    }
}
