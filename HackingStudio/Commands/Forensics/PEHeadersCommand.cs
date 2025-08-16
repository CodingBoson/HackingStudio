using BosonWare.TerminalApp;
using HackingStudio.Core.Forensics;

namespace HackingStudio.Commands.Forensics;

[Command("PEHeaders", Description = "Extracts PE headers in the windows portable executable file.")]
internal sealed class PEHeadersCommand : IForensicsCommand, ICommand
{
    public Task Execute(string filePath)
    {
        var analyzer = new BinaryAnalyzer();

        analyzer.ExtractPEHeaders(filePath);

        return Task.CompletedTask;
    }
}
