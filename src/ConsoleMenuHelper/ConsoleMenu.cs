using System;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleMenuHelper.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleMenuHelper
{
    /// <summary>This is the main class for starting up the console menu.  You should call the following items in this order:
    /// AddDependencies (if you have any), AddMenuItemViaReflection (passing in the assembly that contains them),
    /// and finally DisplayMenuAsync (with the name of the main menu)</summary>
    public class ConsoleMenu
    {
        private bool _dependenciesAdded;
        private ServiceProvider _serviceProvider;
        private IConsoleMenuRepository _menuRepository;
        private IConsoleMenuController _menuController;
       
        /// <summary>Show the first menu.</summary>
        /// <param name="menuName">The first main's name</param>
        public async Task DisplayMenuAsync(string menuName)
        {
            await DisplayMenuAsync(menuName, string.Empty);
        }

        /// <summary>Show the first menu.</summary>
        /// <param name="menuName">The first main's name</param>
        /// <param name="title">Menu title</param>
        public async Task DisplayMenuAsync(string menuName, string title)
        {
            if (_dependenciesAdded == false) AddDependencies(null);

            await _menuController.DisplayMenuAsync(menuName, title);
        }

        /// <summary>Finds all the classes and interfaces that are decorated with the <see cref="ConsoleMenuItemAttribute"/> attribute.
        /// If it's a class, it makes sure they also implements the <see cref="IConsoleMenuItem"/> interface.  If it's an interface,
        /// it makes sure that it inherits from the <see cref="IConsoleMenuItem"/> interface.</summary>
        public ConsoleMenu AddMenuItemViaReflection(Assembly assembly)
        {
            if (_dependenciesAdded == false) AddDependencies(null);

            _menuRepository.AddMenuItems(assembly);

            return this;
        }

        /// <summary>Registers user dependencies with the dependency inject framework.</summary>
        /// <param name="userCallBackMethod">The method that should be called.</param>
        public void AddDependencies(Action<IServiceCollection> userCallBackMethod)
        {
            if (_dependenciesAdded) throw new ArgumentException("Please call the AddDependencies method ONLY once!");

            var collection = new ServiceCollection();

            RegisterInternalDependencies(collection);
         
            userCallBackMethod?.Invoke(collection); // Register user dependencies

            _serviceProvider = collection.BuildServiceProvider();
            
            // Create these here (as opposed to the RegisterInternalDependencies method)
            // so that user can override them if desired!
            _menuRepository = _serviceProvider.GetService<IConsoleMenuRepository>();
            _menuController = _serviceProvider.GetService<IConsoleMenuController>();

            _dependenciesAdded = true;
        }

        /// <summary>Register internal dependencies</summary>
        /// <param name="collection">Service collection</param>
        private void RegisterInternalDependencies(IServiceCollection collection)
        {
            collection.AddSingleton<IConsoleMenuRepository, ConsoleMenuRepository>();
            collection.AddSingleton<IConsoleMenuController, ConsoleMenuController>();
            

            collection.AddTransient<IExitConsoleMenuItem, ExitConsoleMenuItem>();
            collection.AddTransient<IConsoleCommand, ConsoleCommand>();
            collection.AddSingleton<IPromptHelper, PromptHelper>();
        }
    }
}