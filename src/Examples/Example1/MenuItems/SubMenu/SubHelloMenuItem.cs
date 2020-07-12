using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using Microsoft.Extensions.Configuration;

namespace Example1
{
    [ConsoleMenuItem("Hello2", Data = "This is some data")]
    public class SubHelloMenuItem : IConsoleMenuItem
    {
        private readonly IConfiguration _configuration;

        public SubHelloMenuItem(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            Console.WriteLine($"Hello2 from work item 1: {_configuration["TestMessage"]}");
            if (string.IsNullOrWhiteSpace(AttributeData) == false)
            {
                Console.WriteLine($"Optional data found: {AttributeData}");
            }
            return await Task.FromResult(new ConsoleMenuItemResponse(false, false));
        }

        public string ItemText => "Sub Menu ever 1!";

        /// <summary>Optional data from the attribute.</summary>
        public string AttributeData { get; set; }
    }
}