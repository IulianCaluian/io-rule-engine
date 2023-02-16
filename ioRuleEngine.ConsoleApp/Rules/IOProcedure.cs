
namespace ioRulesEngine.ConsoleApp.Rules
{
    public class IOProcedure
    {
        private List<IOAction> Actions { get; set; }

        public IOProcedure()
        {
            Actions = new List<IOAction>();
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

    }
}
