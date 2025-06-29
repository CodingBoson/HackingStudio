using Cocona;
using Figgle;
using HackingStudio.Core;

namespace HackingStudio.CLI;

[HasSubCommands(typeof(PluginCommands), "plugin", Description = "Manage plugins for HackingStudio.")]
public sealed class PluginCommandsSection { }

public sealed class PluginCommands
{
	[Command("list", Description = "Displays currently installed plugins. (Alias: ls)", Aliases = ["ls"])]
	public static async Task List(bool noAsciiArt = false)
	{
		foreach (var plugin in await PluginManager.GetPluginsAsync()) {
			if (noAsciiArt) {
				Console.WriteLine($"{plugin.Name} -> {plugin.Command}.({plugin.Description})");

				continue;
			}

			var name = FiggleFonts.Slant.Render(plugin.Name);

			SmartConsole.WriteLine(name, ConsoleColor.Green);

			Console.WriteLine($"	Name:        {plugin.Name}");
			Console.WriteLine($"	Exec:        {plugin.Command}");
			Console.WriteLine($"	Description: {plugin.Description}");
		}
	}

	[Command("install", Description = "Install a plugin.")]
	public static async Task Install([Option('n')] string name, 
		[Option('c')] string command,
		[Option('s')] string source, 
		[Option('d')] string description = "")
	{
		if (await PluginManager.IsInstalledAsync(name)) {
			var answer = SmartConsole.
				ReadLine("The specified plugin is already installed. Do you want to update it?(Y/n)").
				ToLower();

			if (answer != "y" && answer != "yes") {
				return;
			}

			await PluginManager.UninstallAsync(name);
		}

		var plugin = new Plugin() { Name = name, Description = description, Command = command };

		await PluginManager.InstallAsync(plugin, source);
	}

	[Command("uninstall", Description = "Uninstall a plugin.")]
	public static async void Uninstall([Argument(Description = "The name of the plugin to uninstall.")] string name)
	{
		if (!await PluginManager.UninstallAsync(name)) {
			SmartConsole.LogError("The specified plugin is not installed.");
		}
	}

	[Command("rename", Description = "Renames a plugin.")]
	public static async Task Rename([Argument(Description = "The name of the plugin to rename.")] string name,
		[Option('n')] string newName)
	{
		if (!await PluginManager.IsInstalledAsync(name)) {
			SmartConsole.LogError("The specified plugin is not installed.");

			return;
		}

		await PluginManager.RenameAsync(pluginName: name, newName: newName);
	}

	[Command("run", Description = "Run a plugin.")]
	public static async Task Run([Argument(Description = "The name of the plugin to run.")] string name,
		[Option('a')] string arguments = "Arguments to pass to the plugin.")
	{
		if (await PluginManager.GetPluginAsync(name) is not Plugin plugin) {
			var bestCandidates = await PluginManager.SearchAsync(name);

			if (bestCandidates.Count > 0) {
				SmartConsole.LogError($"No plugin named {name} is installed. Did you mean? {string.Join(", ", bestCandidates.Select(x => x.Name))}");
			}
			else {
				SmartConsole.LogError($"No plugin named {name} is installed.");
			}

			return;
		}

		using var environment = new PluginEnvironment(plugin);

		await environment.ExecuteAsync(arguments);
	}

	//[Command("PATH", Description = "Displays the directory where plugins are installed.")]
	[Command("GetPath", Description = "Displays the directory where plugins are installed.")]
	public static void PluginPath() => Console.Write(PluginManager.PluginsPath);
}