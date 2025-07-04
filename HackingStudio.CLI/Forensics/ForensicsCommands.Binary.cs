using Cocona;
using HackingStudio.Core;

namespace HackingStudio.CLI.Forensics;

public partial class ForensicsCommands
{
    // Analyze the binary file and filter strings based on character set flags
    [Command("dumpstrs", Description = "Dumps all ascii or unicode printable characters in the provided binary file.")]
    public static void DumpStrings([Argument] string filePath,
        [Option('l')] int minStringLength = 4,
        [Option] bool ascii = false,
        [Option] bool unicode = false,
        [Option('u')] bool unique = false)
    {
        var analyzer = new BinaryAnalyzer();
        var strings = analyzer.DumpStrings(filePath, ascii, unicode, minStringLength);

        foreach (var str in unique ? strings.Distinct() : strings) {
            Console.WriteLine(str);
        }
    }

    // Detect hardcoded IP addresses and URLs in the binary file
    [Command("dumpurls", Description = "Dumps all urls in the provided binary file.")]
    public static void DumpUrls([Argument] string filePath)
    {
        var analyzer = new BinaryAnalyzer();
        var results = analyzer.DumpUrls(filePath);

        foreach (var result in results) {
            Console.WriteLine(result);
        }
    }

    // Generate a hex dump
    [Command("hexdump", Description = "Displays a hex view of the provided binary file.")]
    public static void HexDump([Argument] string filePath)
    {
        var analyzer = new BinaryAnalyzer();

        analyzer.HexDump(filePath);
    }

    // Generate a string map
    [Command("stringmap", Description = "Displays a map of all ASCII printable characters with there offsets.",
        Aliases = ["strmap"])]
    public static void StringMap([Argument] string filePath)
    {
        var analyzer = new BinaryAnalyzer();

        analyzer.StringMap(filePath);
    }

    // Extract PE Headers
    [Command("PEHeaders", Description = "Extracts PE headers in the windows portable executable file.")]
    public static void ExtractPEHeaders([Argument] string filePath)
    {
        var analyzer = new BinaryAnalyzer();
        analyzer.ExtractPEHeaders(filePath);
    }

    // Extract Metadata
    [Command("PEMetadata", Description = "Extracts metadata about the specified binary file.")]
    public static void ExtractMetadata([Argument] string filePath)
    {
        var analyzer = new BinaryAnalyzer();

        analyzer.ExtractMetadata(filePath);
    }
}