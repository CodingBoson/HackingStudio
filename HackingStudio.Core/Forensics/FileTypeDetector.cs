using System.Text;

namespace HackingStudio.Core.Forensics;

public class FileTypeDetector
{
    /// <summary>
    ///     Tries to identify a file based on its header (magic numbers) for a variety of file formats:
    ///     Images, PDF, ZIP, Audio, Video, Application binaries, and plain text.
    /// </summary>
    /// <param name="filePath">The path to the file to analyze.</param>
    /// <returns>A string description of the file type, or "Unknown file type" if not recognized.</returns>
    public static string GetFileType(string filePath)
    {
        if (!File.Exists(filePath))
            return "Error: File not found.";

        // Read up to 64 bytes. This buffer should be enough for most magic numbers.
        var buffer = new byte[64];
        using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read)) {
            fs.ReadExactly(buffer);
        }

        switch (buffer.Length) {
            // --- Image/Document Checks (existing ones) ---
            // PNG: 89 50 4E 47 0D 0A 1A 0A
            case >= 8 when
                buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E &&
                buffer[3] == 0x47 && buffer[4] == 0x0D && buffer[5] == 0x0A &&
                buffer[6] == 0x1A && buffer[7] == 0x0A:
                return "PNG image";
            // JPEG: Check for ID3 tag or MP header (supports several variants)
            case >= 3 when
                buffer[0] == 0x49 && buffer[1] == 0x44 && buffer[2] == 0x33:
                return "JPEG/Mp3 audio? (ID3 tag detected)"; // Note: "ID3" can also be on MP3 files
            case >= 3 when
                buffer[0] == 0xFF &&
                (buffer[1] == 0xFB || buffer[1] == 0xFA || buffer[1] == 0xF3):
                // Most likely an MP3 frame header
                return "MP3 audio";
            // GIF: "GIF8" as the first 4 bytes (works for GIF87a and GIF89a)
            case >= 4 when
                buffer[0] == 0x47 && buffer[1] == 0x49 &&
                buffer[2] == 0x46 && buffer[3] == 0x38:
                return "GIF image";
            // PDF: "%PDF"
            case >= 4 when
                buffer[0] == 0x25 && buffer[1] == 0x50 &&
                buffer[2] == 0x44 && buffer[3] == 0x46:
                return "PDF document";
            // ZIP: "PK "
            case >= 4 when
                buffer[0] == 0x50 && buffer[1] == 0x4B &&
                (buffer[2] == 0x03 || buffer[2] == 0x05 || buffer[2] == 0x07) &&
                (buffer[3] == 0x04 || buffer[3] == 0x06 || buffer[3] == 0x08):
                return "ZIP archive";
            // --- Audio Files ---
            // WAV: "RIFF" at beginning and "WAVE" at offset 8
            case >= 12: {
                var riff = Encoding.ASCII.GetString(buffer, 0, 4);
                var wave = Encoding.ASCII.GetString(buffer, 8, 4);
                if (riff == "RIFF" && wave == "WAVE") {
                    return "WAV audio";
                }
                break;
            }
        }

        switch (buffer.Length) {
            // FLAC: "fLaC" at beginning (0x66 0x4C 0x61 0x43)
            case >= 4 when
                buffer[0] == 0x66 && buffer[1] == 0x4C &&
                buffer[2] == 0x61 && buffer[3] == 0x43:
                return "FLAC audio";
            // --- Video Files ---
            // MP4 / MOV: At offset 4, "ftyp" is common.
            case >= 12: {
                var ftyp = Encoding.ASCII.GetString(buffer, 4, 4);
                if (ftyp == "ftyp") {
                    // Read the next 4 characters for the brand identifier.
                    var brand = Encoding.ASCII.GetString(buffer, 8, 4);
                    if (brand.StartsWith("mp4", StringComparison.OrdinalIgnoreCase) ||
                        brand.StartsWith("M4A", StringComparison.OrdinalIgnoreCase) ||
                        brand.StartsWith("isom", StringComparison.OrdinalIgnoreCase) ||
                        brand.StartsWith("avc1", StringComparison.OrdinalIgnoreCase)) {
                        return "MP4 video";
                    }
                    return brand.Contains("qt", StringComparison.OrdinalIgnoreCase) ? "MOV video" : "ISO Base Media File (MP4/MOV)";
                }
                break;
            }
        }

        // AVI: "RIFF" and at offset 8 "AVI " (with a trailing space)
        if (buffer.Length >= 12) {
            var riff = Encoding.ASCII.GetString(buffer, 0, 4);
            var avi = Encoding.ASCII.GetString(buffer, 8, 4);
            if (riff == "RIFF" && avi == "AVI ") {
                return "AVI video";
            }
        }
        switch (buffer.Length) {
            // MKV: Matroska (typically starts with 1A 45 DF A3)
            case >= 4 when
                buffer[0] == 0x1A && buffer[1] == 0x45 &&
                buffer[2] == 0xDF && buffer[3] == 0xA3:
                return "MKV video (Matroska)";
            // --- Application Binaries ---
            // Windows PE / DOS Executable: "MZ" at the very beginning.
            case >= 2 when
                buffer[0] == 0x4D && buffer[1] == 0x5A:
                return "Windows executable (PE/DOS)";
            // Linux ELF binaries: 0x7F 'E' 'L' 'F'
            case >= 4 when
                buffer[0] == 0x7F && buffer[1] == 0x45 &&
                buffer[2] == 0x4C && buffer[3] == 0x46:
                return "Linux ELF binary";
            // MacOS Mach-O binaries:
            // 32-bit big-endian: FE ED FA CE
            case >= 4 when buffer[0] == 0xFE && buffer[1] == 0xED &&
                           buffer[2] == 0xFA && buffer[3] == 0xCE:
                return "MacOS Mach-O binary (32-bit BE)";
            // 32-bit little-endian: CE FA ED FE
            case >= 4 when buffer[0] == 0xCE && buffer[1] == 0xFA &&
                           buffer[2] == 0xED && buffer[3] == 0xFE:
                return "MacOS Mach-O binary (32-bit LE)";
            // 64-bit big-endian: FE ED FA CF
            case >= 4 when buffer[0] == 0xFE && buffer[1] == 0xED &&
                           buffer[2] == 0xFA && buffer[3] == 0xCF:
                return "MacOS Mach-O binary (64-bit BE)";
            // 64-bit little-endian: CF FA ED FE
            case >= 4 when buffer[0] == 0xCF && buffer[1] == 0xFA &&
                           buffer[2] == 0xED && buffer[3] == 0xFE:
                return "MacOS Mach-O binary (64-bit LE)";
        }

        // --- Plain Text Files ---
        // Check if the first bytes are all text (allowing CR, LF and tab)
        var isText = true;
        var printableCount = 0;
        // If the file is less than 1 byte, say it’s empty text.
        var lengthToCheck = Math.Min(buffer.Length, 64);
        for (var i = 0; i < lengthToCheck; i++) {
            var b = buffer[i];
            // Allow common control characters: tab (9), LF (10), CR (13)
            if (!(b == 9 || b == 10 || b == 13 || b >= 32 && b <= 126)) {
                isText = false;
                break;
            }
            printableCount++;
        }
        // If the bulk of the bytes are text, consider it a plain text file.
        if (!isText || printableCount <= 0)
            return "Unknown file type";

        // Also check for UTF-8 BOM (EF BB BF) if available
        if (lengthToCheck >= 3 &&
            buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF) {
            return "UTF-8 text file (with BOM)";
        }
        return "Plain text file";

    }
}
