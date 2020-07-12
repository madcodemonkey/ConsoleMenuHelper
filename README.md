# Work in progress....

# ConsoleMenuHelper
Used to create .NET Core console applications with menus.  It uses reflection to find the menu items and builds the menus for you so that you can focus on the work the menu item does.  It was originally developed to help me test Azure Functions.

# Setup 
The ConsoleMenuHelper uses reflection to find menu items.  It then groups them be menu name that is specified in the ConsoleMenuItem attribute.

## Step 1: Console
Create a .NET Core console application.

## Step 2: Install Nuget
Install the [ConsoleMenuHelper Nuget package](https://www.nuget.org/packages/ConsoleMenuHelper) into your .NET project.

## Step 3: Create some menu options
You must decorate each menu item with the ConsoleMenuItem attribute and specify:
1. Menu name "Hello1" in the example below.  Now all classes with "Hello1" are in the same menu.
1. Optional: Add an option menu number.  If not specifed, one will be generated (sorted by ItemText and assigned sequentially)
1. Optional: Add a data string.  It will be assigned to the AttributeData property once instatiated.
```csharp
using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;


[ConsoleMenuItem("Hello1", 2)]
public class DeveloperQuestionMenuItem : IConsoleMenuItem
{
	private readonly IPromptHelper _promptHelper;
	public DeveloperQuestionMenuItem(IPromptHelper promptHelper)
	{
		_promptHelper = promptHelper;
	}

	public async Task<ConsoleMenuItemResponse> WorkAsync()
	{
		if (_promptHelper.GetYorN("Are you a developer?"))
		{
			Console.WriteLine("Use the source Luke!");
		}
		else
		{
			Console.WriteLine("Just curious, huh.");
		}

		Console.WriteLine("------------------------------------");

		return await Task.FromResult(new ConsoleMenuItemResponse(false, false));
	}

	public string ItemText => "Ask me a question!";

	/// <summary>Optional data from the attribute.</summary>
	public string AttributeData { get; set; }
}
```


In this example, a menu item belongs to TWO different menus, Hello1 and Hello2
```csharp
using System;
using System.Threading.Tasks;
using ConsoleMenuHelper;


[ConsoleMenuItem("Hello1", 1)]
[ConsoleMenuItem("Hello2")]
public class WhatIsYourNameMenuItem : IConsoleMenuItem
{
	private readonly IPromptHelper _promptHelper;

	public WhatIsYourNameMenuItem(IPromptHelper promptHelper)
	{
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

	/// <summary>Optional data from the attribute.</summary>
	public string AttributeData { get; set; }
}
```


## Step 4: Show some menus
Follow these sub-steps
1. Create an instance of ConsoleMenu
1. Optional: Add your dependencies.  You don't have to specify any, but if you do you MUST do it before calling AddMenuItemViaReflection
1. Call AddMenuItemViaReflection and specify the assembly that contains your menu items.  If its the console application itself, use Assembly.GetExecutingAssembly()
1. Call  DisplayMenuAsync with the name of your main menu ("Hello1" below).

```csharp
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleMenuHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
	static async Task Main()
	{
		try
		{
			var menu = new ConsoleMenu();

			menu.AddDependencies(AddMyDependencies);
			menu.AddMenuItemViaReflection(Assembly.GetExecutingAssembly()); 
		
			await menu.DisplayMenuAsync("Hello1");

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
```

Notes
- In this example, I'm using IConfiguration and ingecting it as a singleton so that my menu items can gain access to things in "appsettings.json"
- You'll notice that I register ONE menu item manually. I did this because I attributed an INTERFACE and I need to specify the concrete class; otherwise, I'll get an exception.
- Finally, I show an example over overriding a built in menu item called IExitConsoleMenuItem.  It is injected into every menu to give you an option to exit with a value of zero.


