using System.Threading.Tasks;

namespace ConsoleMenuHelper
{
    /// <summary>This menu item is used to EXIT a menu and return to the main menu or exist the program entirely.</summary>
    public class ExitConsoleMenuItem : IExitConsoleMenuItem
    {
        /// <summary>Return the necessary response to the controller to exit.</summary>
        public Task<ConsoleMenuItemResponse> WorkAsync()
        {
            return Task.FromResult(new ConsoleMenuItemResponse(true, true));
        }

        /// <summary>The text to display on the menu.</summary>
        public string ItemText => "Exit";


        /// <summary>Optional data from the attribute.</summary>
        public string AttributeData { get; set; }
    }
}