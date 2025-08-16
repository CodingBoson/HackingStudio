using BosonWare.TerminalApp;
using HackingStudio.Core.Forensics;

namespace HackingStudio.Commands.Forensics;

[Command("PEMetadata", Description = "Extracts metadata about the specified binary file.")]
internal sealed class PEMetadataCommand : IForensicsCommand, ICommand
{
    public Task Execute(string filePath)
    {
        var analyzer = new BinaryAnalyzer();

        analyzer.ExtractMetadata(filePath);

        return Task.CompletedTask;
    }
}
