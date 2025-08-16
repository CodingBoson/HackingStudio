using BosonWare.TerminalApp;

namespace HackingStudio.Commands.FS;

[Command("cd",  Description = "Changes the current directory path.")]
internal sealed class ChangeDirectoryCommand : IFSCommand, ICommand
{
    public Task Execute(string path)
    {
        if (!Directory.Exists(path)) {
            SmartConsole.LogError($"Directory not found: {path}");
            
            return Task.CompletedTask;
        }
        
        Environment.CurrentDirectory = path;
        
        return Task.CompletedTask;
    }
}
