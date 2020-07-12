using System;

namespace ConsoleMenuHelper
{
    public class MenuItem
    {
        public Type TheType { get; set; }
        public ConsoleMenuItemAttribute Attribute { get; set; }
        public IConsoleMenuItem Item { get; set; }
        public int ItemNumber { get; set; }
    }
}