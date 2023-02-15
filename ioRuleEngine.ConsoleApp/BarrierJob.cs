using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRuleEngine.ConsoleApp
{
    public class BarrierJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // Get the current time
            var now = DateTime.Now;

            // If the time is between 9am and 6pm, open the barrier
            if (now.Hour >= 9 && now.Hour < 18)
            {
                OpenBarrier();
            }
            // Otherwise, close the barrier
            else
            {
                CloseBarrier();
            }

            return Task.CompletedTask;
        }

        private void OpenBarrier()
        {
            // Execute the command to open the barrier
            // This will depend on the specific details of your system
            Console.WriteLine("Opening the barrier.");
        }

        private void CloseBarrier()
        {
            // Execute the command to close the barrier
            // This will depend on the specific details of your system
            Console.WriteLine("Closing the barrier.");
        }
    }
}
