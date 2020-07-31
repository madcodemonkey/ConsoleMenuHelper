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
        private readonly Stack<ConsoleMenuWrapper> _menuStack = new Stack<ConsoleMenuWrapper>();

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
            await DisplayMenuAsync(menuName, string.Empty, BreadCrumbType.None);
        }

        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        /// <param name="title">The title of the menu</param>
        public async Task DisplayMenuAsync(string menuName, string title)
        {
            await DisplayMenuAsync(menuName, title, BreadCrumbType.None);
        }

        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        /// <param name="title">The title of the menu</param>
        /// <param name="breadCrumbType">The type of breadcrumb trail you would like to see.</param>
        public async Task DisplayMenuAsync(string menuName, string title, BreadCrumbType breadCrumbType)
        {
            var menuList = _menuRepository.LoadMenus(menuName);

            var instantiatedMenuList = _menuRepository.CreateMenuItems(menuList);

            var newMenu = new ConsoleMenuWrapper
            {
                Title = title,
                BreadCrumbTitle = BuildBreadCrumbTrail(breadCrumbType, title),
                MenuItems = instantiatedMenuList
            };

            _menuStack.Push(newMenu);

            bool clearScreen = true;

            while(true)
            {
                ShowOneMenu(clearScreen);

                ConsoleMenuItemResponse response = await DoWorkAsync();
                
                if (response.ExitMenu) break;
                
                clearScreen = response.ClearScreen;
            }

            _menuStack.Pop();
        }

        /// <summary>Creates a breadcrumb trail.</summary>
        /// <param name="breadCrumbType">The type of breadcrumb trail you would like to see.</param>
        /// <param name="title">The title of the menu</param>
        private string BuildBreadCrumbTrail(BreadCrumbType breadCrumbType, string title)
        {          
            if (breadCrumbType == BreadCrumbType.None) return string.Empty;
            
            var currentMenu = _menuStack.Count > 0 ? _menuStack.Peek() : null;

            if (currentMenu == null || string.IsNullOrWhiteSpace(currentMenu.Title)) return title;

            if (breadCrumbType == BreadCrumbType.ParentOnly || string.IsNullOrWhiteSpace(currentMenu.BreadCrumbTitle))
            {
                return $"{currentMenu.Title} > {title}";
            }

            return $"{currentMenu.BreadCrumbTitle} > {title}";
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

                var worker =  currentMenuItems.MenuItems.FirstOrDefault(w => w.ItemNumber == userChoice.Value);
                if (worker == null)
                {
                    ShowOneMenu(true);
                    _console.WriteLine("*******Please enter a valid number*******");
                }
                else
                {
                    result = await worker.Item.WorkAsync();
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
          
            var currentMenu = _menuStack.Peek();

            string title = string.IsNullOrWhiteSpace(currentMenu.BreadCrumbTitle) ? currentMenu.Title : currentMenu.BreadCrumbTitle;

            if (string.IsNullOrWhiteSpace(title) == false)
            {
                _console.WriteLine(title);
            }

            foreach (var menuItem in currentMenu.MenuItems)
            {
                _console.WriteLine($"{menuItem.ItemNumber}. {menuItem.Item.ItemText}");
            }
            
            _console.WriteLine("Hit enter to clear the screen and refresh the menu");
        }
    }
}
