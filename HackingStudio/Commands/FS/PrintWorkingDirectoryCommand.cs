using BosonWare.TerminalApp;

namespace HackingStudio.Commands.FS;

[Command("pwd",  Description = "Prints the current directory path.")]
internal sealed class PrintWorkingDirectoryCommand : IFSCommand, ICommand
{
    public Task Execute(string path)
    {
        TUIConsole.WriteLine($"{Environment.CurrentDirectory}");
        
        return Task.CompletedTask;
    }
}
