using System;

namespace ConsoleMenuHelper
{
    /// <summary>An attribute used to find worker classes.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class ConsoleMenuItemAttribute : Attribute
    {
        /// <summary>Constructor</summary>
        public ConsoleMenuItemAttribute(string menuName)
        {
            MenuName = menuName;
        }

        /// <summary>Constructor</summary>
        public ConsoleMenuItemAttribute(string menuName, int itemNumber)
        {
            MenuName = menuName;
            ItemNumber = itemNumber;
        }

        /// <summary>The name of the parent menu that should hold this item.</summary>
        public string MenuName { get; set; }

        /// <summary>An optional, selection number.  The menu will use this to determine the order that an item appears
        /// in the menu.  If NOT specified, we sort by the <see cref="MenuName"/> property</summary>
        public int ItemNumber { get; set; }
        
        /// <summary>An optional, data string that will passed into the <see cref="IConsoleMenuItem"/> interfaces AttributeData property.</summary>
        public string Data { get; set; }
    }
}