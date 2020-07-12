using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleMenuHelper
{
    /// <summary>Controls the actual display of the menu and takes in user menu selections.</summary>
    public interface IConsoleMenuController
    {
        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        Task DisplayMenuAsync(string menuName);
      
        
        /// <summary>Finds all the classes and interfaces that are decorated with the <see cref="ConsoleMenuItemAttribute"/> attribute.
        /// If it's a class, it makes sure they also implements the <see cref="IConsoleMenuItem"/> interface.  If it's an interface,
        /// it makes sure that it inherits from the <see cref="IConsoleMenuItem"/> interface.</summary>
        void FindWorkers(Assembly assembly);
    }
}