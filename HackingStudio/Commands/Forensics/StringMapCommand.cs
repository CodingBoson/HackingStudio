using BosonWare.TerminalApp;
using HackingStudio.Core.Forensics;

namespace HackingStudio.Commands.Forensics;

[Command("stringmap", Description = "Displays a map of all ASCII printable characters with there offsets.")]
internal sealed class StringMapCommand : IForensicsCommand, ICommand
{
    public Task Execute(string filePath)
    {
        var analyzer = new BinaryAnalyzer();

        analyzer.StringMap(filePath);

        return Task.CompletedTask;
    }
}
