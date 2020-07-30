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
        static async Task Main()
        {
            try
            {
                var menu = new ConsoleMenu();

                menu.AddDependencies(AddMyDependencies);
                menu.AddMenuItemViaReflection(Assembly.GetExecutingAssembly()); 
            
                await menu.DisplayMenuAsync("Hello1", "Main");

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Hit enter to exit");
            Console.ReadLine();
        }

        static void AddMyDependencies(IServiceCollection serviceCollection)
        {

            // IConfiguration requires: Microsoft.Extensions.Configuration NuGet package
            // AddJsonFile requires:    Microsoft.Extensions.Configuration.Json NuGet package
            // https://stackoverflow.com/a/46437144/97803
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();

            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddTransient<IAustinPowersMenuItem, AustinPowersMenuItem>();

            // Overriding a built in component
            // serviceCollection.AddSingleton<IExitConsoleMenuItem,ExitConsoleOverrideMenuItem>();
        }
    }
}
