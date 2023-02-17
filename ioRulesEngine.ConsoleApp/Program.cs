using System.Collections.Generic;
using ioRulesEngine.ConsoleApp.Devices;
using ioRulesEngine.ConsoleApp.Rules;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;


/*
// Load the rules from a JSON file
string rulesFilePath = "rules.json";

Console.WriteLine("Rules engine.");

using (var streamReader = new StreamReader(rulesFilePath))
using (var jsonReader = new JsonTextReader(streamReader))
{
    var serializer = new JsonSerializer();
    var rules = serializer.Deserialize<List<IORule>>(jsonReader) ?? new List<IORule>();

    var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
    await scheduler.Start();

    var rulesEngine = new RulesEngine(rules, new DeviceProcessor());

    await rulesEngine.StartAsync();
}

*/

Console.WriteLine("Hello world!");

List<IORule> rules = new List<IORule>()
{
    new IORule(
        new IORuleTrigger() { TriggerSource = TriggerSourceEnum.TimeEvent, TriggerEventData = new TimeEventTriggerEventData() { Hour = 21, Minute = 10 } },
        new IOProcedure(new List<IOAction>()  {  new IOAction() { ActionType = IOActionType.ControllBarrierCommand }  })
    )
};

RulesEngine rulesEngine = new RulesEngineBuilder().Rules(rules).Build();

await rulesEngine.StartAsync();

Console.WriteLine("Press any key to stop:");
Console.ReadKey();
await rulesEngine.StopAsync();

Console.WriteLine("Goodbye!");
await Task.Delay(2000);






