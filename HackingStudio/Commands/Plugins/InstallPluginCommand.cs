using BosonWare.TerminalApp;
using CommandLine;
using HackingStudio.Core;

namespace HackingStudio.Commands.Plugins;

[Command("install", Description = "Install a plugin")]
internal sealed class InstallPluginCommand : Command<InstallPluginCommand.Options>, IPluginCommand
{
    internal sealed class Options
    {
        [Option('n', "name", Required = true, HelpText = "The Plugin's Name")]
        public string Name { get; set; } = "";
        
        [Option('d', "description", Required = true, HelpText = "The Plugin's Description")]
        public string Description { get; set; } = "";
        
        [Option('c', "command", Required = true, HelpText = "The Plugin's Command")]
        public string Command { get; set; } = "";
        
        [Option('s', "source", Required = true, HelpText = "The Plugin's Source")]
        public string Source { get; set; } = "";

        public void Deconstruct(
            out string name, 
            out string description, 
            out string command, 
            out string source)
        {
            name = Name;
            description = Description;
            command = Command;
            source = Source;
        }
    }
    
    public override async Task Execute(Options options)
    {
        var (name, description, command, source) = options;
        
        if (await PluginManager.IsInstalledAsync(name)) {
            var answer = SmartConsole.
                ReadLine("The specified plugin is already installed. Do you want to update it?(Y/n)").
                ToLower();

            if (answer != "y" && answer != "yes") {
                return;
            }

            await PluginManager.UninstallAsync(name);
        }

        var plugin = new Plugin { Name = name, Description = description, Command = command };

        await PluginManager.InstallAsync(plugin, source);
    }
}
