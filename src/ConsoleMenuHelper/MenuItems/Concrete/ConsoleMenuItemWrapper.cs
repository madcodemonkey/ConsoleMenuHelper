using System;

namespace ConsoleMenuHelper
{
    /// <summary>Wraps the raw data found during the assembly search
    /// and when instantiated, holds the instantiated instance.</summary>
    public class ConsoleMenuItemWrapper
    {
        /// <summary>The type of the object that was decorated with the <see cref="ConsoleMenuItemAttribute"/> attribute</summary>
        public Type TheType { get; set; }

        /// <summary>The attribute data</summary>
        public ConsoleMenuItemAttribute Attribute { get; set; }

        /// <summary>Null initially, but once the menu is displayed it will hold the instantiated menu item.</summary>
        public IConsoleMenuItem Item { get; set; }

        /// <summary>The number, which is used for selecting the menu item in the list of menu items.</summary>
        public int ItemNumber { get; set; }
    }
}