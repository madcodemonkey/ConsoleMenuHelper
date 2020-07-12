using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;

namespace Example1
{

    [ConsoleMenuItem("Hello2")]
    public interface IAustinPowersMenuItem :  IConsoleMenuItem { }


    public class AustinPowersMenuItem : IAustinPowersMenuItem
    {
        private readonly IPromptHelper _promptHelper;

        public AustinPowersMenuItem(IPromptHelper promptHelper)
        {
            _promptHelper = promptHelper;
        }

        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            if (_promptHelper.GetYorN("Are you Austin Powers?"))
            {
                Console.WriteLine("Yea, baby!");
            }
            else
            {
                Console.WriteLine("I like honesty!");
            }

            Console.WriteLine("----------------------------------------------");

            return await Task.FromResult(new ConsoleMenuItemResponse(false, false));
        }

        public string ItemText => "Austin Powers interface test item!";

        /// <summary>Optional data from the attribute.</summary>
        public string AttributeData { get; set; }
    }
}