using BosonWare.TerminalApp;
using CommandLine;
using HackingStudio.Core;

namespace HackingStudio.Commands.Plugins;

[Command("run", Description = "Executes a plugin")]
internal sealed class RunPluginCommand : Command<RunPluginCommand.Options>, IPluginCommand
{
    internal sealed class Options
    {
        [Value(0, Required = true, HelpText = "The Plugin's Name")]
        public string Name { get; set; } = "";

        [Option('a', Required = false, HelpText = "The arguments to pass to the plugin.")]
        public string Arguments { get; set; } = "";

        public void Deconstruct(out string name, out string arguments)
        {
            name = Name;
            arguments = Arguments;
        }
    }
    
    public override async Task Execute(Options options)
    {
        var (name, arguments) = options;
        
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
}
