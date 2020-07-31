namespace ConsoleMenuHelper
{
    /// <summary>The type of breadcrumb trail you would like to see.</summary>
    public enum BreadCrumbType
    {
        /// <summary>Don't show a breadcrumb trail</summary>
        None = 0,

        /// <summary>Show the parent and child only</summary>
        ParentOnly,

        /// <summary>Look at the parents breadcrumb trail and add to it.
        /// If the parent doesn't have a breadcrumb trail then just use the parent's title
        /// to build one.</summary>
        Concatenate, 

    }
}
