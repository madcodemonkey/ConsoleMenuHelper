using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using ConsoleMenuHelper.Helpers;
using Microsoft.Extensions.Configuration;

namespace Example1
{
    [ConsoleMenuItem("Hello1", 1)]
    [ConsoleMenuItem("Hello2")]
    public class WhatIsYourNameMenuItem : IConsoleMenuItem
    {
        private readonly IConfiguration _configuration;
        private readonly IPromptHelper _promptHelper;

        public WhatIsYourNameMenuItem(IConfiguration configuration, IPromptHelper promptHelper)
        {
            _configuration = configuration;
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