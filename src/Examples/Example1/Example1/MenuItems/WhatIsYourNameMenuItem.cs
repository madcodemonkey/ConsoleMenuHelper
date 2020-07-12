using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using ConsoleMenuHelper.Helpers;

namespace Example1
{
    [ConsoleMenuItem("Hello1", 1)]
    [ConsoleMenuItem("Hello2")]
    public class WhatIsYourNameMenuItem : IConsoleMenuItem
    {
        private readonly IPromptHelper _promptHelper;

        public WhatIsYourNameMenuItem(IPromptHelper promptHelper)
        {
            _promptHelper = promptHelper;
        }

        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            string name = _promptHelper.GetText("What's your name?", false, true);

            Console.WriteLine($"Hello, {name}");

            Console.WriteLine("-------------------------------");

            return await Task.FromResult(new ConsoleMenuItemResponse(false, false));
        }

        public string ItemText => "What your name?";
    }
}