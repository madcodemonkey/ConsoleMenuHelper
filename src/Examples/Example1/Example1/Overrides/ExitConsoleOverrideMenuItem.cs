using System.Threading.Tasks;
using ConsoleMenuHelper;

namespace Example1
{
    public class ExitConsoleOverrideMenuItem : IExitConsoleMenuItem
    {
        public Task<ConsoleMenuItemResponse> WorkAsync()
        {
            return Task.FromResult(new ConsoleMenuItemResponse(true, true));
        }

        public string ItemText => "Exit NOW!!";


        /// <summary>Optional data from the attribute.</summary>
        public string AttributeData { get; set; }
    }
}
