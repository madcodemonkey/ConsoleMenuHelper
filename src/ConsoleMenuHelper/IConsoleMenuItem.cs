using System.Threading.Tasks;

namespace ConsoleMenuHelper
{
    /// <summary>Interface used by all workers.  Worker do work when their number is pressed
    /// in the main menu of the console application.</summary>
    public interface IConsoleMenuItem
    {
        /// <summary>Causes the worker to take control of the output.  Returns true if the
        /// screen should be cleared and main menu shown again on the screen; otherwise, false.</summary>
        Task<ConsoleMenuItemResponse> WorkAsync();

        /// <summary>This is the text used beside the number in the console application's menu</summary>
        string ItemText { get; }
    }
}