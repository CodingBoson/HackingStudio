using BosonWare.TerminalApp;
using HackingStudio.Core;
using HackingStudio.Core.Forensics;

namespace HackingStudio.Commands.Forensics;

[Command("file", Description = "Displays a file's information.")]
internal sealed class FileCommand : IForensicsCommand, ICommand
{
    public Task Execute(string path)
    {
        if (!File.Exists(path)) {
            SmartConsole.LogError($"File '{path}' does not exist.");

            return Task.CompletedTask;
        }

        using (var fileStream = File.OpenRead(path)) {
            Console.WriteLine(fileStream.Length > 1024
                ? $"Size: {fileStream.Length} | {IOUtility.PrettySize(fileStream.Length)}"
                : $"Size: {IOUtility.PrettySize(fileStream.Length)}");
        }

        var type = FileTypeDetector.GetFileType(path);

        Console.WriteLine($"FileType: {type}");

        return Task.CompletedTask;
    }
}
