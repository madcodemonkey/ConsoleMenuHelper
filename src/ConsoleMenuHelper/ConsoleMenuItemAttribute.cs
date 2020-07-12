using System;

namespace ConsoleMenuHelper
{
    /// <summary>An attribute used to find worker classes.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class ConsoleMenuItemAttribute : Attribute
    {
        public ConsoleMenuItemAttribute(string menuName)
        {
            MenuName = menuName;
        }

        public ConsoleMenuItemAttribute(string menuName, int itemNumber)
        {
            MenuName = menuName;
            ItemNumber = itemNumber;
        }

        /// <summary>The name of the parent menu that should hold this item.</summary>
        public string MenuName { get; set; }

        /// <summary>The number that should be used to select the item.  This is optional</summary>
        public int ItemNumber { get; set; }
    }
}