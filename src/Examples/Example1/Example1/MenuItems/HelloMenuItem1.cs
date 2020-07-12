using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using Microsoft.Extensions.Configuration;

namespace Example1
{
    [ConsoleMenuItem("Hello1", 1)]
    [ConsoleMenuItem("Hello2")]
    public class HelloMenuItem1 : IConsoleMenuItem
    {
        private readonly IConfiguration _configuration;

        public HelloMenuItem1(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> WorkAsync()
        {
            Console.WriteLine($"Hello from work item 1: {_configuration["TestMessage"]}");
            return await Task.FromResult(false);
        }

        public string MenuItemText => "Best ever 1!";
    }
}