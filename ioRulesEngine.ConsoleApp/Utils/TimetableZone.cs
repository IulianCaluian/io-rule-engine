using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Utils
{
    public class TimetableZone
    {
        public DateTime StartOfTheDay { get; set; }
        public DateTime EndOfTheDay { get; set; }

        private TimetableZone()
        {

        }

        internal static TimetableZone Default()
        {
            return new TimetableZone();
        }
    }
}
