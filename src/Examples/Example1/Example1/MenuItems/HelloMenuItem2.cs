using System.Threading.Tasks;
using ConsoleMenuHelper;

namespace Example1
{
    [ConsoleMenuItem("Hello1", 2)]
    public class HelloMenuItem2 : IConsoleMenuItem
    {
        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            System.Console.WriteLine("Hello from work item 2");
            return await Task.FromResult(new ConsoleMenuItemResponse(false, false));
        }

        public string ItemText => "Best ever 2!";
    }
}