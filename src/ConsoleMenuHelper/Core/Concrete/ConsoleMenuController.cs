using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleMenuHelper.Core;

namespace ConsoleMenuHelper
{
    /// <summary>Controls the actual display of the menu and takes in user menu selections.</summary>
    public class ConsoleMenuController : IConsoleMenuController
    {
        private readonly IConsoleMenuRepository _menuRepository;
        private readonly IConsoleCommand _console;
        private readonly IPromptHelper _promptHelper;
        private readonly Stack<List<ConsoleMenuItemWrapper>> _menuStack = new Stack<List<ConsoleMenuItemWrapper>>();

        /// <summary>Constructor</summary>
        public ConsoleMenuController(IConsoleMenuRepository menuRepository, IConsoleCommand console, IPromptHelper promptHelper)
        {
            _menuRepository = menuRepository;
            _console = console;
            _promptHelper = promptHelper;
        }

        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        public async Task DisplayMenuAsync(string menuName)
        {
            var menuList = _menuRepository.LoadMenus(menuName);

            var instantiatedMenuList = _menuRepository.CreateMenuItems(menuList);

            _menuStack.Push(instantiatedMenuList);

            ShowOneMenu(true);

            ConsoleMenuItemResponse response;

            do
            {
                response = await DoWorkAsync();
            }
            while (response.ExitMenu == false);

            _menuStack.Pop();
        }
        
        /// <summary>Displays the menu and attempts to get a menu item select from the user.
        /// If the user selects an item, it works is performed on that selected item.</summary>
        private async Task<ConsoleMenuItemResponse> DoWorkAsync()
        {
            int? userChoice = _promptHelper.GetNumber(null, 1);
       
            var result = new ConsoleMenuItemResponse(false, true);
            
            if (userChoice.HasValue)
            {
                var currentMenuItems = _menuStack.Peek();

                var worker =  currentMenuItems.FirstOrDefault(w => w.ItemNumber == userChoice.Value);
                if (worker == null)
                {
                    ShowOneMenu(true);
                    _console.WriteLine("*******Please enter a valid number*******");
                }
                else
                {
                    result = await worker.Item.WorkAsync();
                    ShowOneMenu(result.ClearScreen);
                }
            }
            else
            {
                ShowOneMenu(true);
                _console.WriteLine("*******Please enter a valid number*******");
            }

            return result;
        }

        /// <summary>Shows one menu.</summary>
        /// <param name="clearScreen">Indicates if you would like to clear the screen.</param>
        private void ShowOneMenu(bool clearScreen)
        {
            if (clearScreen)
            {
                _console.Clear();
            }
          
            var currentMenuItems = _menuStack.Peek();

            foreach (var menuItem in currentMenuItems)
            {
                _console.WriteLine($"{menuItem.ItemNumber}. {menuItem.Item.ItemText}");
            }
            
            _console.WriteLine("Hit enter to clear the screen and refresh the menu");
        }
    }
}
