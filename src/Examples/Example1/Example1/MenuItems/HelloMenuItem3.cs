using System.Threading.Tasks;
using ConsoleMenuHelper;

namespace Example1
{
    [ConsoleMenuItem("Hello1")]
    public class HelloMenuItem3 : IConsoleMenuItem
    {
        private readonly IConsoleMenuController _menuController;

        public HelloMenuItem3(IConsoleMenuController menuController)
        {
            _menuController = menuController;
        }

        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            await _menuController.DisplayMenuAsync("Hello2");
            return await Task.FromResult(new ConsoleMenuItemResponse(false, true));
        }

        public string ItemText => "Show Hello 2 menu!";
    }
}