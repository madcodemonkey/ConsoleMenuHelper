using System.Threading.Tasks;

namespace ConsoleMenuHelper
{
    /// <summary>Controls the actual display of the menu and takes in user menu selections.</summary>
    public interface IConsoleMenuController
    {
        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        Task DisplayMenuAsync(string menuName);

        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        /// <param name="title">The title of the menu</param>
        Task DisplayMenuAsync(string menuName, string title);
        
        /// <summary>Displays a menu an prompts the user to select a menu item.</summary>
        /// <param name="menuName">The name of the menu (it's NOT case sensitive)</param>
        /// <param name="title">The title of the menu</param>
        /// <param name="breadCrumbType">The type of breadcrumb trail you would like to see.</param>
        Task DisplayMenuAsync(string menuName, string title, BreadCrumbType breadCrumbType);
    }
}