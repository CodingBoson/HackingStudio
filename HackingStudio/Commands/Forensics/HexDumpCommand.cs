using BosonWare.TerminalApp;
using HackingStudio.Core.Forensics;

namespace HackingStudio.Commands.Forensics;

[Command("hexdump", Description = "Displays a hex view of the provided binary file.")]
internal sealed class HexDumpCommand : IForensicsCommand, ICommand
{
    public Task Execute(string filePath)
    {
        var analyzer = new BinaryAnalyzer();

        analyzer.HexDump(filePath);

        return Task.CompletedTask;
    }
}
