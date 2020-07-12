using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var menu = new ConsoleMenu();

            menu.Register(RegisterMyItems);
            menu.FindWorkers(Assembly.GetExecutingAssembly()); 
            
            await menu.WorkAsync("Hello1");

            await menu.WorkAsync("Hello2");

            Console.WriteLine("Done!");
        }

        static void RegisterMyItems(IServiceCollection serviceCollection)
        {

            // IConfiguration requires: Microsoft.Extensions.Configuration NuGet package
            // AddJsonFile requires:    Microsoft.Extensions.Configuration.Json NuGet package
            // https://stackoverflow.com/a/46437144/97803
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();

            serviceCollection.AddSingleton<IConfiguration>(config);
        }
    }
}
