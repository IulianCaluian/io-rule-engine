using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Devices
{
    public class DeviceProcessor
    {
        public event EventHandler<EventArgs>? InputChanged;
        public event EventHandler<EventArgs>? OutputChanged; 

        public DeviceProcessor() { }    

        public void OnInputChanged()
        {
            InputChanged?.Invoke(this, EventArgs.Empty);    
        }

        public void OnOutputChanged()
        {
            OutputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
