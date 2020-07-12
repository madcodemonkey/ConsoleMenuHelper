using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using ConsoleMenuHelper.Helpers;

namespace Example1
{
    [ConsoleMenuItem("Hello1", 2)]
    public class DeveloperQuestionMenuItem : IConsoleMenuItem
    {
        private readonly IPromptHelper _promptHelper;
        public DeveloperQuestionMenuItem(IPromptHelper promptHelper)
        {
            _promptHelper = promptHelper;
        }

        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            if (_promptHelper.GetYorN("Are you a developer?"))
            {
                Console.WriteLine("Use the source Luke!");
            }
            else
            {
                Console.WriteLine("Just curious, huh.");
            }

            Console.WriteLine("------------------------------------");

            return await Task.FromResult(new ConsoleMenuItemResponse(false, false));
        }

        public string ItemText => "Ask me a question!";
    }
}