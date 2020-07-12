using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using Microsoft.Extensions.Configuration;

namespace Example1
{
    [ConsoleMenuItem("Hello2")]
    public class SubHelloMenuItem1 : IConsoleMenuItem
    {
        private readonly IConfiguration _configuration;

        public SubHelloMenuItem1(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            Console.WriteLine($"Hello2 from work item 1: {_configuration["TestMessage"]}");
            return await Task.FromResult(new ConsoleMenuItemResponse(false, false));
        }

        public string ItemText => "Sub Menu ever 1!";
    }
}