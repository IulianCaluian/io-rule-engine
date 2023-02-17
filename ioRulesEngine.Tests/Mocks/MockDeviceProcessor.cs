using ioRulesEngine.ConsoleApp.Devices;
using ioRulesEngine.ConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.Tests.Mocks
{
    public class MockDeviceProcessor : IDeviceProcessor
    {
        public event AsyncEventHandler<EventArgs>? InputChanged;
        public event AsyncEventHandler<EventArgs>? OutputChanged;

        public async Task InvokeInputEvent()
        {
           await OnInputChanged();
        }

        public async Task InvokeOutputEvent()
        {
           await OnOutputChanged();
        }

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
