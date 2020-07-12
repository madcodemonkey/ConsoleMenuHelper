namespace ConsoleMenuHelper
{
    /// <summary>This is used by ever console menu item to give feedback to the <see cref="IConsoleMenuController"/> on the desired
    /// action to take after it has completed its work.</summary>
    public class ConsoleMenuItemResponse
    {
        /// <summary>Constructor</summary>
        public ConsoleMenuItemResponse() { }

        /// <summary>Constructor</summary>
        public ConsoleMenuItemResponse(bool exitMenu, bool clearScreen)
        {
            ExitMenu = exitMenu;
            ClearScreen = clearScreen;
        }

        /// <summary>Indicates that after this menu item has completed its work, you want to exit the menu.</summary>
        public bool ExitMenu { get; set; }

        /// <summary>Indicates that you want to clear the screen after work has completed.
        /// You usually want to set this to true when going from a sub-menu to a main menu; however,
        /// if it just work results, you may want to leave it on the screen to help the user with their next choice.</summary>
        public bool ClearScreen { get; set; }
    }
}