using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleMenuHelper
{
    public class ConsoleMenu
    {
        private ServiceProvider _serviceProvider;
        protected readonly Dictionary<string, List<MenuItem>> _menus = new Dictionary<string, List<MenuItem>>();

        /// <summary>Constructor</summary>
        public ConsoleMenu()
        {

        }
        
              
        public async Task WorkAsync(string menuName)
        {
            if (_serviceProvider == null) Register(null);

            if (string.IsNullOrWhiteSpace(menuName)) throw new ArgumentException("Please enter a valid menu name.");
            var scrubMenuName = ScrubMenuName(menuName);

            if (_menus.ContainsKey(scrubMenuName) == false) throw new ArgumentException($"{menuName} not found!");
            
            List<MenuItem> instantiatedMenuItems = CreateMenuItems(_menus[scrubMenuName]);

            try
            {
                ShowOneMenu(instantiatedMenuItems, true);

                bool stayInLoop = true;
                while (stayInLoop)
                {
                    string input = Console.ReadLine();
                    stayInLoop = await ProcessMainMenuInput(instantiatedMenuItems, input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Hit enter to exit");
                Console.ReadLine();
            }
        }

        
        public async Task<bool> ProcessMainMenuInput(List<MenuItem> instantiatedMenuItems, string input)
        {

            if (string.IsNullOrWhiteSpace(input))
            {
                ShowOneMenu(instantiatedMenuItems, true);
                return true;
            }

            if (int.TryParse(input, out var numberInput))
            {
                if (numberInput == 0) return false;

                var worker =  instantiatedMenuItems.FirstOrDefault(w => w.ItemNumber == numberInput);
                if (worker == null)
                {
                    Console.Clear();
                    Console.WriteLine("*******Please enter a valid number*******");
                    ShowOneMenu(instantiatedMenuItems, false);
                }
                else
                {
                    bool clearScreenAndShowMainMenu = await worker.Item.WorkAsync();
                    ShowOneMenu(instantiatedMenuItems, clearScreenAndShowMainMenu);
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("*******Please enter a valid number*******");
                ShowOneMenu(instantiatedMenuItems, false);
            }

            Console.WriteLine("--------------------------------");
            return true;

        }

        private void ShowOneMenu(List<MenuItem> instantiatedMenuItems, bool clearScreen)
        {

            if (clearScreen)
            {
                Console.Clear();
            }
            else
            {
                Console.WriteLine("--------------------------------");
            }


            foreach (var menuItem in instantiatedMenuItems)
            {
                Console.WriteLine($"{menuItem.ItemNumber}. {menuItem.Item.MenuItemText}");
            }

            Console.WriteLine("0. Exit");
            Console.WriteLine("Hit enter to clear the screen and refresh the menu");
        }

        private static string ScrubMenuName(string menuName)
        {
            if (string.IsNullOrWhiteSpace(menuName)) return null;
            return menuName.Trim().ToLower();
        }

        private List<MenuItem> CreateMenuItems(List<MenuItem> menu)
        {
            foreach (var menuItem in menu)
            {
                if (menuItem.Item != null) continue;
                
                // Is the user's object registered in DI (e.g., an interface?)
                // If it's not, try to create it and use DI to populate its constructor.
                var userObject = _serviceProvider.GetService(menuItem.TheType) ?? 
                    _serviceProvider.CreateInstance(menuItem.TheType);

                if (userObject == null)
                {
                    throw new ArgumentException($"Could not find the type named '{menuItem.TheType.FullName}' in the DI container and was unable to create it!");
                }
                
                menuItem.Item =  _serviceProvider.CreateInstance(menuItem.TheType) as IConsoleMenuItem;;

                if (menuItem.Item == null)
                {
                    throw new ArgumentException($"The {menuItem.TheType.FullName} does NOT implement the IConsoleMenuItem interface!");
                }
            }
            
            var result = menu.OrderBy(o => o.ItemNumber).ThenBy(o => o.Item.MenuItemText).ToList();

            for (int i = 1; i <= menu.Count; i++)
            {
                var menu1 = result.FirstOrDefault(w => w.ItemNumber == 1);
                if (menu1 != null) continue;
                menu1 = result.FirstOrDefault(w => w.ItemNumber == 0);
                if (menu1 == null) continue;
                menu1.ItemNumber = i;
            }
            
            return menu.OrderBy(o => o.ItemNumber).ThenBy(o => o.Item.MenuItemText).ToList();
        }


        /// <summary>Finds all the classes that have the <see cref="ConsoleMenuItemAttribute"/> attribute,
        /// makes sure they also implement the <see cref="IConsoleMenuItem"/> interface, instantiates an
        /// instance of the worker and passes it back in a list.</summary>
        public ConsoleMenu FindWorkers(Assembly assembly)
        {
            var typesWithHelpAttribute = FindAllClassesThatImplementTheWorkerAttribute(assembly);


            foreach (Type oneType in typesWithHelpAttribute)
            {
                // Make sure it implements the interface too!
                bool implementsTheWorkerInterface = oneType.GetInterfaces().Contains(typeof(IConsoleMenuItem));
                if (implementsTheWorkerInterface == false)
                {
                    throw new ArgumentException($"The {oneType.Name} type is decorated with the ConsoleMenuItemAttribute, but does not implement the IConsoleMenuItem interface! ");
                }

                foreach (Attribute attribute in oneType.GetCustomAttributes(typeof(ConsoleMenuItemAttribute)))
                {
                    ConsoleMenuItemAttribute cmia = attribute as ConsoleMenuItemAttribute;
                    if (cmia == null) continue; // TODO: throw exception!

                    var newItem = new MenuItem()
                    {
                        Attribute = cmia,
                        TheType = oneType,
                        ItemNumber = cmia.ItemNumber
                    };

                    string someMenuName = ScrubMenuName(cmia.MenuName);
                    if (_menus.ContainsKey(someMenuName))
                    {
                        _menus[someMenuName].Add(newItem);
                    }
                    else
                    {
                        _menus.Add(someMenuName, new List<MenuItem> { newItem });
                    }
                }
            }

            return this;
        }

        /// <summary>Finds all classes that implement the <see cref="ConsoleMenuItemAttribute"/> and returns them as a list of types.</summary>
        private static List<Type> FindAllClassesThatImplementTheWorkerAttribute(Assembly assembly)
        {
            // var assembly = Assembly.GetExecutingAssembly();
            
            var typesWithHelpAttribute =
                (from type in assembly.GetTypes()
                    where type.IsDefined(typeof(ConsoleMenuItemAttribute), false)
                    select type).ToList();

            return typesWithHelpAttribute;
        }


        public void Register(Action<IServiceCollection> userCallBackMethod)
        {
            if (_serviceProvider != null) throw new ArgumentException("Please call the Register method ONLY once!");

            var collection = new ServiceCollection();
            // collection.AddSingleton<>();


            userCallBackMethod?.Invoke(collection);

            _serviceProvider = collection.BuildServiceProvider();
        }
    }
}