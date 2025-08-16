using BosonWare.TerminalApp;
using CommandLine;
using HackingStudio.Core.Forensics;
using JetBrains.Annotations;

namespace HackingStudio.Commands.Forensics;

[UsedImplicitly]
internal sealed class DumpStringsOptions
{
    [Value(0, Required = true, MetaName = "filePath", HelpText = "The file path")]
    public string FilePath { get; set; }

    [Option('l')]
    public int MinStringLength { get; set; } = 4;

    [Option(Required = false, HelpText = "")]
    public bool Ascii { get; set; } = false;

    [Option(Required = false, HelpText = "")]
    public bool Unicode { get; set; } = false;

    [Option('u', Required = false, HelpText = "")]
    public bool Unique { get; set; } = false;
}

[Command("dumpstrs", Description = "\"Dumps all ascii or unicode printable characters in the provided binary file.")]
internal sealed class DumpStringsCommand : Command<DumpStringsOptions>, IForensicsCommand
{
    public override Task Execute(DumpStringsOptions options)
    {
        var analyzer = new BinaryAnalyzer();
        var strings = analyzer.DumpStrings(
            options.FilePath,
            options.Ascii,
            options.Unicode,
            options.MinStringLength);

        foreach (var str in options.Unique ? strings.Distinct() : strings) {
            Console.WriteLine(str);
        }

        return Task.CompletedTask;
    }
}
