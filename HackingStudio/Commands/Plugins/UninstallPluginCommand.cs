using BosonWare.TerminalApp;
using HackingStudio.Core;

namespace HackingStudio.Commands.Plugins;

[Command("uninstall", Description = "Uninstall a plugin")]
internal sealed class UninstallPluginCommand : IPluginCommand, ICommand
{
    public async Task Execute(string name)
    {
        if (!await PluginManager.UninstallAsync(name)) {
            SmartConsole.LogError("The specified plugin is not installed.");
        }
    }
}
