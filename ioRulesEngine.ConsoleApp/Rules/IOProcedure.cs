
namespace ioRulesEngine.ConsoleApp.Rules
{
    public class IOProcedure
    {
        private List<IOAction> Actions { get; set; }

        public IOProcedure(List<IOAction>? actions = null)
        {
            Actions = actions ?? new List<IOAction>();
        }

        public void AddAction(IOAction action)
        {
            Actions.Add(action);
        }

        public void RemoveAction(IOAction action)
        {
            if (Actions.Contains(action))
                Actions.Remove(action);
        }

        public async Task Execute()
        {
            foreach(var action in Actions)
            {
                await action.Execute();
            }
        }

    }
}
