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

        [TestMethod]
        public async Task ActivatingTriggerVariable_ShouldExecuteProcedure()
        {
            int triggerVariableNumber = 1;

            IOAction actionToExecute = new IOAction() { ActionType = IOActionType.GenericAction };

            List<IORule> rules = new List<IORule>()
            {
                new IORule(
                    new IORuleTrigger()
                    {
                        TriggerSource = TriggerSourceEnum.TriggerVariable,
                        TriggerEventData = new  TriggerVariableEventData()
                        {
                            TriggerVariableNumber = triggerVariableNumber
                        }
                    },
                    new IOProcedure(new List<IOAction>()
                    {
                        actionToExecute
                    })
                )
            };


            RulesEngine rulesEngine = new RulesEngineBuilder()
                .Rules(rules).Build();

            // Act:
            await rulesEngine.SetTriggerVariable(triggerVariableNumber, true);

            // Asert:
            Assert.IsTrue(actionToExecute.Executed);
        }



        [TestMethod]
        public async Task ExecuteRegisteredProcedureAction_ShouldExecuteSpecificProcedure()
        {
            //Arrange:
            int triggerVariableNumber = 1;
            int registeredProcedureNuumber = 2;
            IOAction actionInRegisteredProcedure = new IOAction() { ActionType = IOActionType.GenericAction };
            Dictionary<int, IOProcedure> registeredProcedures = new Dictionary<int, IOProcedure>() {
                {  registeredProcedureNuumber, new IOProcedure(new List<IOAction>() { actionInRegisteredProcedure }) } 
            };

            List<IORule> rules = new List<IORule>()
            {
                new IORule(
                    new IORuleTrigger()
                    {
                        TriggerSource = TriggerSourceEnum.TriggerVariable,
                        TriggerEventData = new  TriggerVariableEventData()
                        {
                            TriggerVariableNumber = triggerVariableNumber
                        }
                    },
                    new IOProcedure(new List<IOAction>()
                    {
                        new IOAction()
                        {
                            ActionType = IOActionType.ExecuteRegisteredProcedure,
                            ActionEventData = new ExecuteProcedureActionEventData()
                            {
                                ProcedureNumber =  registeredProcedureNuumber
                            }
                        }
                    })
                )
            };

           
            RulesEngine rulesEngine = new RulesEngineBuilder()
                .RegisteredProcedures(registeredProcedures)
                .Rules(rules).Build();

            // Act:
            await rulesEngine.SetTriggerVariable(triggerVariableNumber, true);

            // Asert:
            Assert.IsTrue(actionInRegisteredProcedure.Executed);

        }

        [TestMethod]
        public async Task ProcedureTrigger_ShouldSpecificProcedureAndSequentProcedures()
        {   
            //Arrange:
           

        }
    }
}