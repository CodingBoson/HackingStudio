using BosonWare.TerminalApp;
using Figgle;
using HackingStudio.Core;

namespace HackingStudio.Commands.Plugins;

[Group("plugin", Description = "Plugin commands")]
internal interface IPluginCommand;

[Command("list", Aliases = ["ls"], Description = "List installed plugins")]
internal sealed class ListPluginsCommand : IPluginCommand, ICommand
{
    public async Task Execute(string arguments)
    {
        foreach (var plugin in await PluginManager.GetPluginsAsync()) {
            var name = FiggleFonts.Slant.Render(plugin.Name);

            SmartConsole.WriteLine(name, ConsoleColor.Green);

            Console.WriteLine($"	Name:        {plugin.Name}");
            Console.WriteLine($"	Exec:        {plugin.Command}");
            Console.WriteLine($"	Description: {plugin.Description}");
        }
    }
}
