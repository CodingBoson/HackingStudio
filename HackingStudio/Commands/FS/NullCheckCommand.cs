using BosonWare.TerminalApp;

namespace HackingStudio.Commands.FS;

[Group("fs", Description = "File system commands.")]
internal interface IFSCommand;

[Command("null-check", Description = "Calculates the null ratio for the specified file.")]
internal sealed class NullCheckCommand : IFSCommand, ICommand
{
    public Task Execute(string path)
    {
        var stream = File.OpenRead(path);

        long index = 0;
        long nullBytes = 0;
        while (true) {
            index++;

            var @byte = stream.ReadByte();

            if (@byte == -1) {
                break;
            }

            if (@byte == 0) {
                nullBytes++;
            }

            if ((index + 1) % 150000000 != 0)
                continue;

            var currentNullRatio = (float)((double)index / nullBytes * 100);

            SmartConsole.LogInfo($"NullRatio: {currentNullRatio}%");
            SmartConsole.LogInfo($"Position: {(double)index / stream.Length * 100}%");
        }

        // We cast the double to a float
        var nullRatio = (float)((double)nullBytes / stream.Length * 100);

        SmartConsole.LogInfo($"NullRatio: {nullRatio}%");

        return Task.CompletedTask;
    }
}

[Command("list", Aliases = ["ls"], Description = "Lists files in the current directory.")]
internal sealed class ListFilesCommand : IFSCommand, ICommand
{
    public Task Execute(string arguments)
    {
        foreach (var path in Directory.EnumerateDirectories(Environment.CurrentDirectory)) {
            var attributes = File.GetAttributes(path);
            
            TUIConsole.WriteLine($"[Green]{attributes}[/] {path}");
        }
        foreach (var path in Directory.EnumerateFiles(Environment.CurrentDirectory)) {
            var attributes = File.GetAttributes(path);
            
            TUIConsole.WriteLine($"[Green]{attributes}[/] {path}");
        }
        
        return Task.CompletedTask;
    }
}

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

[Command("pwd",  Description = "Prints the current directory path.")]
internal sealed class PrintWorkingDirectoryCommand : IFSCommand, ICommand
{
    public Task Execute(string path)
    {
        TUIConsole.WriteLine($"{Environment.CurrentDirectory}");
        
        return Task.CompletedTask;
    }
}
