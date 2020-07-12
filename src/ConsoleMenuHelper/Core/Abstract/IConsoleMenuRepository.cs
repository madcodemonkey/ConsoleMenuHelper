using System.Collections.Generic;
using System.Reflection;

namespace ConsoleMenuHelper.Core
{
    /// <summary>Holds and creates menus</summary>
    public interface IConsoleMenuRepository
    {
        /// <summary>Loads a list of menus</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        List<ConsoleMenuItemWrapper> LoadMenus(string menuName);

        /// <summary>Finds all the classes and interfaces that are decorated with the <see cref="ConsoleMenuItemAttribute"/> attribute.
        /// If it's a class, it makes sure they also implements the <see cref="IConsoleMenuItem"/> interface.  If it's an interface,
        /// it makes sure that it inherits from the <see cref="IConsoleMenuItem"/> interface.</summary>
        void AddMenuItems(Assembly assembly);

        /// <summary>Instantiates the menu items using the dependency injection framework.</summary>
        /// <param name="menu">Menu items to create.</param>
        List<ConsoleMenuItemWrapper> CreateMenuItems(List<ConsoleMenuItemWrapper> menu);
    }
}