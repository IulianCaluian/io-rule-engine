using ioRulesEngine.ConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Devices
{
    public class DeviceProcessor : IDeviceProcessor
    {
        public event AsyncEventHandler<EventArgs>? InputChanged;
        public event AsyncEventHandler<EventArgs>? OutputChanged; 

        public DeviceProcessor() { }

        private async Task OnInputChanged()
        {
            if (InputChanged != null)
            {
                await InputChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private async Task OnOutputChanged()
        {
            if (OutputChanged != null)
            {
               await OutputChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
