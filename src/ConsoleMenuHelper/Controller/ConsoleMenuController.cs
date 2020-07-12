using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleMenuHelper.Helpers;

namespace ConsoleMenuHelper
{
    /// <summary>Controls the actual display of the menu and takes in user menu selections.</summary>
    public class ConsoleMenuController : IConsoleMenuController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsoleCommand _console;
        private readonly IPromptHelper _promptHelper;
        private readonly Dictionary<string, List<ConsoleMenuItemWrapper>> _menus = new Dictionary<string, List<ConsoleMenuItemWrapper>>();
        private readonly Stack<List<ConsoleMenuItemWrapper>> _menuQueue = new Stack<List<ConsoleMenuItemWrapper>>();

        /// <summary>Constructor</summary>
        public ConsoleMenuController(IServiceProvider serviceProvider, IConsoleCommand console, IPromptHelper promptHelper)
        {
            _serviceProvider = serviceProvider;
            _console = console;
            _promptHelper = promptHelper;
        }

        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        public async Task DisplayMenuAsync(string menuName)
        {
            if (string.IsNullOrWhiteSpace(menuName))
            {
                throw new ArgumentException("Please enter a valid menu name.");
            }

            var normalizedMenuName = NormalizeMenuName(menuName);

            if (_menus.ContainsKey(normalizedMenuName) == false)
            {
                throw new ArgumentException($"{menuName} not found!");
            }

            _menuQueue.Push(CreateMenuItems(_menus[normalizedMenuName]));

            ShowOneMenu(true);

            ConsoleMenuItemResponse response;

            do
            {
                response = await DoMenuItemWorkAsync();
            }
            while (response.ExitMenu == false);

            _menuQueue.Pop();
        }

        
        /// <summary>Finds all the classes and interfaces that are decorated with the <see cref="ConsoleMenuItemAttribute"/> attribute.
        /// If it's a class, it makes sure they also implements the <see cref="IConsoleMenuItem"/> interface.  If it's an interface,
        /// it makes sure that it inherits from the <see cref="IConsoleMenuItem"/> interface.</summary>
        public void FindWorkers(Assembly assembly)
        {
            var typesWithHelpAttribute = FindAllClassesThatImplementTheWorkerAttribute(assembly);


            foreach (Type oneType in typesWithHelpAttribute)
            {
                // Make sure it implements the interface too!
                bool implementsTheWorkerInterface = oneType.GetInterfaces().Contains(typeof(IConsoleMenuItem));
                if (implementsTheWorkerInterface == false)
                {
                    throw new ArgumentException($"The {oneType.Name} type is decorated with the ConsoleMenuItemAttribute, but does not implement the IConsoleMenuItem interface! ");
                }

                foreach (Attribute attribute in oneType.GetCustomAttributes(typeof(ConsoleMenuItemAttribute)))
                {
                    var itemAttribute = attribute as ConsoleMenuItemAttribute;
                    if (itemAttribute == null) continue; // TODO: throw exception!

                    var newItem = new ConsoleMenuItemWrapper
                    {
                        Attribute = itemAttribute,
                        TheType = oneType,
                        ItemNumber = itemAttribute.ItemNumber
                    };

                    string someMenuName = NormalizeMenuName(itemAttribute.MenuName);
                    if (_menus.ContainsKey(someMenuName))
                    {
                        _menus[someMenuName].Add(newItem);
                    }
                    else
                    {
                        _menus.Add(someMenuName, new List<ConsoleMenuItemWrapper> {newItem});
                    }
                }
            }
        }

        /// <summary>Displays the menu and attempts to get a menu item select from the user.
        /// If the user selects an item, it works is performed on that selected item.</summary>
        private async Task<ConsoleMenuItemResponse> DoMenuItemWorkAsync()
        {
            int? userChoice = _promptHelper.GetNumber(null, 1);
       
            var result = new ConsoleMenuItemResponse(false, true);
            
            if (userChoice.HasValue)
            {
                var currentMenuItems = _menuQueue.Peek();

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
          
            var currentMenuItems = _menuQueue.Peek();

            foreach (var menuItem in currentMenuItems)
            {
                _console.WriteLine($"{menuItem.ItemNumber}. {menuItem.Item.ItemText}");
            }
            
            _console.WriteLine("Hit enter to clear the screen and refresh the menu");
        }


        /// <summary>Instantiates the menu items using the dependency injection framework.</summary>
        /// <param name="menu">Menu items to create.</param>
        private List<ConsoleMenuItemWrapper> CreateMenuItems(List<ConsoleMenuItemWrapper> menu)
        {
            foreach (var menuItem in menu)
            {
                if (menuItem.Item != null) continue;
                
                // Is the user's object registered in DI (e.g., an interface?)
                // If it's not, try to create it and use DI to populate its constructor.
                var userObject = _serviceProvider.GetService(menuItem.TheType);
                
                if (userObject == null && menuItem.TheType.IsInterface)
                {
                    throw new ArgumentException($"You have decorated an interface, '{menuItem.TheType.FullName}', with the " +
                        $"{nameof(ConsoleMenuItemAttribute)} attribute, but you didn't register a concrete class for it in the DI container!");
                }

                if (userObject == null)
                {
                    userObject = _serviceProvider.CreateInstance(menuItem.TheType);
                }

                if (userObject == null)
                {
                    throw new ArgumentException($"Could not find the type named '{menuItem.TheType.FullName}' in the DI container and was unable to create it!");
                }
                
                menuItem.Item = userObject as IConsoleMenuItem;

                if (menuItem.Item == null)
                {
                    throw new ArgumentException($"The {menuItem.TheType.FullName} does NOT implement the IConsoleMenuItem interface!");
                }
            }

            var sortedResult = FixNumberAndSortOrder(menu);

            var exitItem = new ConsoleMenuItemWrapper
            {
                Attribute = new ConsoleMenuItemAttribute(string.Empty), 
                Item = _serviceProvider.GetService(typeof(IExitConsoleMenuItem)) as IExitConsoleMenuItem, 
                ItemNumber = 0,
                TheType = typeof(IExitConsoleMenuItem)
            };

            sortedResult.Add(exitItem);

            return sortedResult;
        }

        /// <summary>Fixes the numbering and then re-sorts the array</summary>
        /// <param name="menu">Menu to fix and sort</param>
        private List<ConsoleMenuItemWrapper> FixNumberAndSortOrder(List<ConsoleMenuItemWrapper> menu)
        {
            var result = menu
                .OrderBy(o => o.ItemNumber)
                .ThenBy(o => o.Item.ItemText)
                .ToList();

            for (int i = 1; i <= menu.Count; i++)
            {
                var menu1 = result.FirstOrDefault(w => w.ItemNumber == i);
                if (menu1 != null) continue;
                menu1 = result.FirstOrDefault(w => w.ItemNumber == 0);
                if (menu1 == null) continue;
                menu1.ItemNumber = i;
            }

            return menu.OrderBy(o => o.ItemNumber).ThenBy(o => o.Item.ItemText).ToList();
        }


        /// <summary>Finds all classes that implement the <see cref="ConsoleMenuItemAttribute"/> and returns them as a list of types.</summary>
        private static List<Type> FindAllClassesThatImplementTheWorkerAttribute(Assembly assembly)
        {
            var typesWithHelpAttribute =
                (from type in assembly.GetTypes()
                    where type.IsDefined(typeof(ConsoleMenuItemAttribute), false)
                    select type).ToList();

            return typesWithHelpAttribute;
        }

        /// <summary>Removes white space, trims and then lower cases the menu name.</summary>
        /// <param name="menuName">The menu name</param>
        private static string NormalizeMenuName(string menuName)
        {
            if (string.IsNullOrWhiteSpace(menuName)) return null;
            return menuName.Trim().ToLower();
        }
    }
}
