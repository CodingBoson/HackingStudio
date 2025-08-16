using System.Runtime.CompilerServices;

namespace HackingStudio.Core;

public static class IOUtility
{
    /// <summary>
    ///     Converts a double into a shortened format with suffixes (B, KB, MB, GB, TB, etc.).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string PrettySize(double number)
    {
        string[] suffixes = {
            "B", "KB", "MB", "GB", "TB"
        };

        var suffixIndex = 0;

        var absNumber = Math.Abs(number);

        // Loop until the absolute value is less than 1000 or we run out of suffixes
        while (absNumber >= 1000 && suffixIndex < suffixes.Length - 1) {
            absNumber /= 1000;
            suffixIndex++;
        }

        // Format with one decimal place if necessary (e.g., 1.2K) and reapply the sign if needed
        var formatted = absNumber.ToString("0.####") + suffixes[suffixIndex];

        return (number < 0 ? "-" : "") + formatted;
    }

    public static long ParseSize(string size)
    {
        const long KB_Mul = 1024;
        const long MB_Mul = KB_Mul * 1024;
        const long GB_Mul = MB_Mul * 1024;

        if (size.Length > 2 && size.EndsWith("KB", StringComparison.OrdinalIgnoreCase)) {
            var num = double.Parse(size[..^2]);

            return (long)(num * KB_Mul);
        }
        if (size.Length > 2 && size.EndsWith("MB", StringComparison.OrdinalIgnoreCase)) {
            var num = double.Parse(size[..^2]);

            return (long)(num * MB_Mul);
        }
        if (size.Length > 2 && size.EndsWith("GB", StringComparison.OrdinalIgnoreCase)) {
            var num = double.Parse(size[..^2]);

            return (long)(num * GB_Mul);
        }

        return long.Parse(size);
    }
}
