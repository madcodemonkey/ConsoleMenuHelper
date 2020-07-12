using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleMenuHelper
{
    /// <summary>This is the main class for starting up the console menu.  You should call the following items in this order:
    /// RegisterDependencies (if you have any), FindWorkers (passing in the assembly that contains them),
    /// and finally DisplayMenuAsync (with the name of the main menu)</summary>
    public class ConsoleMenu
    {
        private bool _dependenciesRegistered;
        private ServiceProvider _serviceProvider;
        private IConsoleMenuController _menuController;
       
        /// <summary>Show the first menu.</summary>
        /// <param name="menuName">The first main's name</param>
        public async Task DisplayMenuAsync(string menuName)
        {
            if (_dependenciesRegistered == false) RegisterDependencies(null);

            await _menuController.DisplayMenuAsync(menuName);
        }

        /// <summary>Finds all the classes and interfaces that are decorated with the <see cref="ConsoleMenuItemAttribute"/> attribute.
        /// If it's a class, it makes sure they also implements the <see cref="IConsoleMenuItem"/> interface.  If it's an interface,
        /// it makes sure that it inherits from the <see cref="IConsoleMenuItem"/> interface.</summary>
        public ConsoleMenu FindWorkers(Assembly assembly)
        {
            if (_dependenciesRegistered == false) RegisterDependencies(null);

            _menuController.FindWorkers(assembly);

            return this;
        }

        /// <summary>Registers user dependencies with the dependency inject framework.</summary>
        /// <param name="userCallBackMethod">The method that should be called.</param>
        public void RegisterDependencies(Action<IServiceCollection> userCallBackMethod)
        {
            if (_dependenciesRegistered) throw new ArgumentException("Please call the RegisterDependencies method ONLY once!");

            var collection = new ServiceCollection();

            RegisterInternalDependencies(collection);
         
            userCallBackMethod?.Invoke(collection); // Register user dependencies

            _serviceProvider = collection.BuildServiceProvider();

            _menuController = _serviceProvider.GetService<IConsoleMenuController>();

            _dependenciesRegistered = true;
        }

        /// <summary>Register internal dependencies</summary>
        /// <param name="collection">Service collection</param>
        private void RegisterInternalDependencies(IServiceCollection collection)
        {
            collection.AddSingleton<IConsoleMenuController, ConsoleMenuController>();
        }
    }
}