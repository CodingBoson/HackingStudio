using BosonWare.TerminalApp;

namespace HackingStudio.Commands.FS;

[Command("list", Aliases = ["ls"], Description = "Lists files in the current directory.")]
internal sealed class ListFilesCommand : IFSCommand, ICommand
{
    public Task Execute(string arguments)
    {
        foreach (var path in Directory.EnumerateDirectories(Environment.CurrentDirectory)) {
            var attributes = File.GetAttributes(path);
            
            TUIConsole.WriteLine($"[Green]{attributes}[/] {Path.GetRelativePath(Environment.CurrentDirectory, path)}");
        }
        foreach (var path in Directory.EnumerateFiles(Environment.CurrentDirectory)) {
            var attributes = File.GetAttributes(path);
            
            TUIConsole.WriteLine($"[Green]{attributes}[/] {Path.GetRelativePath(Environment.CurrentDirectory, path)}");
        }
        
        return Task.CompletedTask;
    }
}
