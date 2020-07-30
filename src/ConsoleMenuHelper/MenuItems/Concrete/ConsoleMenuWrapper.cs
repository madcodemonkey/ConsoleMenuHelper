using System.Collections.Generic;

namespace ConsoleMenuHelper
{
    /// <summary>A wrapper around a group of menu items.</summary>
    public class ConsoleMenuWrapper
    {
        /// <summary>The menu title (without a breadcrumb trail)</summary>
        public string Title { get; set; }

        /// <summary>The breadcrumb trail (includes <see cref="Title"/>)</summary>
        public string BreadCrumbTitle { get; set; }

        /// <summary>All the menu items associated with this menu.</summary>
        public List<ConsoleMenuItemWrapper> MenuItems { get; set; }
    }
}