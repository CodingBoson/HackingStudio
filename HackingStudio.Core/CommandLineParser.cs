using System.Text;

namespace HackingStudio.Core;

public static class CommandLineParser
{
    public static string[] SplitCommandLine(string commandLine)
    {
        List<string> parts = [];
        var part = new StringBuilder();
        var inQuotes = false;
        char? quoteChar = null;

        for (int i = 0; i < commandLine.Length; i++) {
            char c = commandLine[i];

            if ((c == '"' || c == '\'') && (i == 0 || commandLine[i - 1] != '\\')) {
                if (inQuotes && c == quoteChar) {
                    inQuotes = false;
                    quoteChar = null;
                }
                else if (!inQuotes) {
                    inQuotes = true;
                    quoteChar = c;
                }
                else {
                    part.Append(c);
                }
            }
            else if (c == ' ' && !inQuotes) {
                if (part.Length > 0) {
                    parts.Add(part.ToString());
                    part.Clear();
                }
            }
            else {
                part.Append(c);
            }
        }

        if (part.Length > 0) {
            parts.Add(part.ToString());
        }

        return [.. parts];
    }
}