using System.Threading.Tasks;
using ConsoleMenuHelper;

namespace Example1
{
    [ConsoleMenuItem("Hello1", 2)]
    public class WorkMenuItem2 : IConsoleMenuItem
    {
        public async Task<bool> WorkAsync()
        {
            System.Console.WriteLine("Hello from work item 2");
            return await Task.FromResult(false);
        }

        public string MenuItemText => "Best ever 2!";
    }
}