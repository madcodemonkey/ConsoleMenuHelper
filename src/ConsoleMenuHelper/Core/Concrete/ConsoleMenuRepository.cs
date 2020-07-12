using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleMenuHelper.Extensions;

namespace ConsoleMenuHelper.Core
{
    /// <summary>Holds and creates menus</summary>
    public class ConsoleMenuRepository : IConsoleMenuRepository
    {
        private readonly Dictionary<string, List<ConsoleMenuItemWrapper>> _menus = new Dictionary<string, List<ConsoleMenuItemWrapper>>();
        private readonly IServiceProvider _serviceProvider;

        /// <summary>Constructor</summary>
        public ConsoleMenuRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>Loads a list of menus</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        public List<ConsoleMenuItemWrapper> LoadMenus(string menuName)
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

            return _menus[normalizedMenuName];
        }


        /// <summary>Finds all the classes and interfaces that are decorated with the <see cref="ConsoleMenuItemAttribute"/> attribute.
        /// If it's a class, it makes sure they also implements the <see cref="IConsoleMenuItem"/> interface.  If it's an interface,
        /// it makes sure that it inherits from the <see cref="IConsoleMenuItem"/> interface.</summary>
        public void AddMenuItems(Assembly assembly)
        {
            var typesWithHelpAttribute = assembly.HelpFindEverythingDecoratedWithThisAttribute(typeof(ConsoleMenuItemAttribute));


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

        /// <summary>Instantiates the menu items using the dependency injection framework.</summary>
        /// <param name="menu">Menu items to create.</param>
        public List<ConsoleMenuItemWrapper> CreateMenuItems(List<ConsoleMenuItemWrapper> menu)
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

                // Assign the optional data to the now instantiated object.
                menuItem.Item.AttributeData = menuItem.Attribute.Data;
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

        /// <summary>Removes white space, trims and then lower cases the menu name.</summary>
        /// <param name="menuName">The menu name</param>
        private static string NormalizeMenuName(string menuName)
        {
            if (string.IsNullOrWhiteSpace(menuName)) return null;
            return menuName.Trim().ToLower();
        }

    }
}
