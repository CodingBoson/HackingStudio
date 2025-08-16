using BosonWare.TerminalApp;
using HackingStudio.Core.Forensics;

namespace HackingStudio.Commands.Forensics;

[Command("dumpurls", Description = "Dumps all urls in the provided binary file.")]
internal sealed class DumpUrlsCommand : IForensicsCommand, ICommand
{
    public Task Execute(string filePath)
    {
        var analyzer = new BinaryAnalyzer();
        var results = analyzer.DumpUrls(filePath);

        foreach (var result in results) {
            Console.WriteLine(result);
        }

        return Task.CompletedTask;
    }
}
