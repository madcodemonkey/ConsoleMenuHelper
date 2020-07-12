using System.Threading.Tasks;
using ConsoleMenuHelper;

namespace Example1
{
    [ConsoleMenuItem("Hello1")]
    public class LaunchSubMenuItem : IConsoleMenuItem
    {
        private readonly IConsoleMenuController _menuController;

        public LaunchSubMenuItem(IConsoleMenuController menuController)
        {
            _menuController = menuController;
        }

        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            await _menuController.DisplayMenuAsync("Hello2");
            return await Task.FromResult(new ConsoleMenuItemResponse(false, true));
        }

        public string ItemText => "Show Hello 2 menu!";

        
        /// <summary>Optional data from the attribute.</summary>
        public string AttributeData { get; set; }
    }
}