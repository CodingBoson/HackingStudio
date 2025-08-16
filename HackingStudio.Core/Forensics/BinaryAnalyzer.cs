using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace HackingStudio.Core.Forensics;

public partial class BinaryAnalyzer
{
    /// <summary>
    ///     Dumps strings from a binary file with optional ASCII and Unicode filters.
    /// </summary>
    /// <param name="filePath">The path to the binary file.</param>
    /// <param name="asciiOnly">If true, only ASCII strings are dumped.</param>
    /// <param name="unicodeOnly">If true, only Unicode strings are dumped.</param>
    /// <param name="minStringLength">The minimum length of strings to be dumped.</param>
    /// <returns>An enumerable of strings found in the binary file.</returns>
    public IEnumerable<string> DumpStrings(string filePath, bool asciiOnly, bool unicodeOnly, int minStringLength)
    {
        if (!asciiOnly && !unicodeOnly) {
            asciiOnly = true;
        }

        var strings = new List<string>();
        var fileBytes = File.ReadAllBytes(filePath);
        var currentString = new StringBuilder();

        foreach (var b in fileBytes) {
            if (asciiOnly && b is >= 32 and <= 126) // ASCII printable characters
            {
                currentString.Append((char)b);
            }
            else if (unicodeOnly && b is >= 0x20 and <= 0x7E or >= 0xA0) // Unicode
            {
                currentString.Append((char)b);
            }
            else {
                if (currentString.Length >= minStringLength) {
                    strings.Add(currentString.ToString());
                }
                currentString.Clear();
            }
        }

        if (currentString.Length >= minStringLength) {
            strings.Add(currentString.ToString());
        }

        return strings;
    }

    /// <summary>
    ///     Detects hardcoded IP addresses and URLs in a binary file.
    /// </summary>
    /// <param name="filePath">The path to the binary file.</param>
    /// <returns>An enumerable of detected IP addresses and URLs.</returns>
    public IEnumerable<string> DumpUrls(string filePath)
    {
        var results = new List<string>();

        var fileBytes = File.ReadAllBytes(filePath);
        var fileContent = Encoding.ASCII.GetString(fileBytes);

        foreach (Match match in IPRegex().Matches(fileContent)) {
            results.Add($"IP: {match.Value}");
        }

        foreach (Match match in UrlRegex().Matches(fileContent)) {
            results.Add($"URL: {match.Value}");
        }

        return results;
    }

    /// <summary>
    ///     Extracts PE headers from a binary file and prints them to the console.
    /// </summary>
    /// <param name="filePath">The path to the binary file.</param>
    public void ExtractPEHeaders(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var peReader = new PEReader(stream);

        if (!peReader.HasMetadata) {
            Console.WriteLine("No metadata found in the file.");
            return;
        }

        Console.WriteLine($"File: {filePath}");

        var peHeaders = peReader.PEHeaders;
        var peHeader = peHeaders.PEHeader!;

        Console.WriteLine("\nPE Headers:");
        Console.WriteLine($"  Machine: {peHeaders.CoffHeader.Machine}");
        Console.WriteLine($"  Number of Sections: {peHeaders.CoffHeader.NumberOfSections}");
        Console.WriteLine($"  TimeDateStamp: {peHeaders.CoffHeader.TimeDateStamp}");
        Console.WriteLine($"  ImageBase: {peHeader.ImageBase:X}");
        Console.WriteLine($"  EntryPointAddress: {peHeader.AddressOfEntryPoint:X}");

        Console.WriteLine("\nSection Headers:");
        foreach (var section in peHeaders.SectionHeaders) {
            Console.WriteLine($"  Section: {section.Name}");
            Console.WriteLine($"    Virtual Size: {section.VirtualSize}");
            Console.WriteLine($"    Virtual Address: {section.VirtualAddress:X}");
            Console.WriteLine($"    Size of Raw Data: {section.SizeOfRawData}");
        }
    }

    /// <summary>
    ///     Extracts metadata from a binary file and prints it to the console.
    /// </summary>
    /// <param name="filePath">The path to the binary file.</param>
    public void ExtractMetadata(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var peReader = new PEReader(stream);

        if (!peReader.HasMetadata) {
            Console.WriteLine("No metadata found in the file.");
            return;
        }

        var peHeaders = peReader.PEHeaders;
        Console.WriteLine("\nFile Metadata:");
        Console.WriteLine($"  Target Machine: {peHeaders.CoffHeader.Machine}");
        Console.WriteLine($"  Timestamp: {peHeaders.CoffHeader.TimeDateStamp}");
        Console.WriteLine($"  Image Base Address: {peHeaders.PEHeader!.ImageBase:X}");
        Console.WriteLine($"  Number of Sections: {peHeaders.CoffHeader.NumberOfSections}");
    }

    /// <summary>
    ///     Prints a hex dump of a binary file to the console.
    /// </summary>
    /// <param name="filePath">The path to the binary file.</param>
    public void HexDump(string filePath)
    {
        var fileBytes = File.ReadAllBytes(filePath);
        const int bytesPerLine = 16;

        for (var i = 0; i < fileBytes.Length; i += bytesPerLine) {
            Console.Write($"{i:X8}  "); // Offset in file

            // Print hex values
            for (var j = 0; j < bytesPerLine; j++) {
                Console.Write(i + j < fileBytes.Length
                    ? $"{fileBytes[i + j]:X2} "
                    : "   "); // Padding for incomplete lines
            }

            Console.Write(" ");

            // Print ASCII values
            for (var j = 0; j < bytesPerLine; j++) {
                if (i + j >= fileBytes.Length) continue;

                var b = fileBytes[i + j];

                Console.Write(b is >= 32 and <= 126 ? (char)b : '.');
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    ///     Prints strings with their offsets from a binary file to the console.
    /// </summary>
    /// <param name="filePath">The path to the binary file.</param>
    public void StringMap(string filePath)
    {
        var fileBytes = File.ReadAllBytes(filePath);
        var stringBuilder = new StringBuilder();
        const int minLength = 4;

        for (var i = 0; i < fileBytes.Length; i++) {
            var b = fileBytes[i];

            if (b is >= 32 and <= 126) // Printable characters
            {
                stringBuilder.Append((char)b);
            }
            else {
                if (stringBuilder.Length >= minLength) {
                    Console.WriteLine($"Offset {i - stringBuilder.Length:X8}: {stringBuilder}");
                }
                stringBuilder.Clear();
            }
        }

        if (stringBuilder.Length >= minLength) {
            Console.WriteLine($"Offset {fileBytes.Length - stringBuilder.Length:X8}: {stringBuilder}");
        }
    }

    [GeneratedRegex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b")]
    private static partial Regex IPRegex();

    [GeneratedRegex(@"(https?|ftp):\/\/[^\s/$.?#].[^\s]*")]
    private static partial Regex UrlRegex();
}
