using ioRulesEngine.ConsoleApp.Devices;
using ioRulesEngine.ConsoleApp.Rules;
using ioRulesEngine.Tests.Mocks;

namespace ioRulesEngine.Tests
{
    [TestClass]
    public class RulesEngine_RulesTests
    {
        [TestMethod]
        public async Task InputActivatedFromDevice_ShouldExecuteSpecificAction()
        {
            //Arrange:
            var actionTriggeredOnInput = new IOAction() { ActionType = IOActionType.ControllBarrierCommand };
            var actionTriggeredOnOutput = new IOAction() { ActionType = IOActionType.ControllBarrierCommand };
            List<IORule> rules = new List<IORule>()
            { 
                new IORule(
                    new IORuleTrigger() { TriggerSource = TriggerSourceEnum.Input },
                    new IOProcedure(new List<IOAction>() { actionTriggeredOnInput })
                ) ,
                new IORule(
                    new IORuleTrigger() { TriggerSource = TriggerSourceEnum.Output },
                    new IOProcedure(new List<IOAction>() { actionTriggeredOnOutput })
                )
            }; 

            MockDeviceProcessor mockDevice = new MockDeviceProcessor();
            RulesEngine rulesEngine = new RulesEngineBuilder().Rules(rules).DeviceProcessor(mockDevice).Build();

            //Act:
            await rulesEngine.StartAsync();
            await mockDevice.InvokeInputEvent();

            //Assert:
            Assert.IsTrue(actionTriggeredOnInput.Executed);
            Assert.IsFalse(actionTriggeredOnOutput.Executed);


        }
    }
}