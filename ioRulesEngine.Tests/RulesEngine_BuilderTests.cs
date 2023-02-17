using ioRulesEngine.ConsoleApp.Rules;
using ioRulesEngine.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.Tests
{
    [TestClass]
    public class RulesEngine_BuilderTests
    {
        [TestMethod]
        public void InputActivatedFromDevice_ShouldExecuteSpecificAction()
        {
            //Arrange:
            var builder = new RulesEngineBuilder();
            Assert.ThrowsException<ArgumentException>(() => builder.Build());
        }
    }
}
