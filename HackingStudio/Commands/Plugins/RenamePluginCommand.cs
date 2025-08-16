using BosonWare.TerminalApp;
using CommandLine;
using HackingStudio.Core;

namespace HackingStudio.Commands.Plugins;

[Command("rename", Description = "Rename a plugin")]
internal sealed class RenamePluginCommand : Command<RenamePluginCommand.Options>, IPluginCommand
{
    internal sealed class Options
    {
        [Option('n', "name", Required = true, HelpText = "The Plugin's Name")]
        public string Name { get; set; } = "";

        [Option("new-name", Required = true, HelpText = "The new name")]
        public string NewName { get; set; } = "";
    }
    
    public override async Task Execute(Options options)
    {
        if (!await PluginManager.IsInstalledAsync(options.Name)) {
            SmartConsole.LogError("The specified plugin is not installed.");

            return;
        }

        await PluginManager.RenameAsync(pluginName: options.Name, newName: options.NewName);
    }
}
