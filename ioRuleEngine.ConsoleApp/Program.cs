using System.Collections.Generic;
using ioRuleEngine.ConsoleApp;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;


// Load the rules from a JSON file
string rulesFilePath = "rules.json";

using (var streamReader = new StreamReader(rulesFilePath))
using (var jsonReader = new JsonTextReader(streamReader))
{
    var serializer = new JsonSerializer();
    var rules = serializer.Deserialize<List<BarrierRule>>(jsonReader);

    var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
    scheduler.Start();

    var ruleEngine = new BarrierRuleEngine(rules, scheduler);
    ruleEngine.StartAsync();
}



