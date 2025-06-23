using System.Diagnostics;
using System.Threading.Tasks;
using Cocona;
using HackingStudio.Core;

namespace HackingStudio.CLI.Forensics;

[HasSubCommands(typeof(FSCommands), "fs", Description = "File system commands.")]
public sealed class FSCommandsSection { }

public sealed class FSCommands
{
    [Command("alloc", Description = "Allocates space for a new file.")]
    public static async Task Allocate([Option('p')] string path,
        [Option('s', Description = "The size to allocate. (Supports size notation)")] string size,
        [Option('r', Description = "Fill the file with random bytes instead of null ones.")] bool random = false)
    {
        ShutdownManager.Subscribe();

        var watch = Stopwatch.StartNew();

        long sizeInBytes = IOUtility.ParseSize(size);

        byte[] bytes = new byte[sizeInBytes];

        if (random) {
            Random.Shared.NextBytes(bytes);
        }
        else {
            bytes.AsSpan().Clear();
        }

        SmartConsole.LogInfo($"Created array in {watch.Elapsed.TotalSeconds}sec");

        watch.Restart();

        await File.WriteAllBytesAsync(path, bytes);

        SmartConsole.LogInfo($"Allocated {size} in {watch.Elapsed.TotalSeconds}sec");
    }

    [Command("split", Description = "Splits a large file into smaller chunks.")]
    public static async Task Split([Option('p')] string path,
        [Option('s', Description = "The size of each chunk. (Supports size notation)")] string size)
    {
        ShutdownManager.Subscribe();

        var filename = Path.GetFileNameWithoutExtension(path);
        var stream = File.OpenRead(path);

        int chunkIndex = 0;

        long sizeInBytes = IOUtility.ParseSize(size);

        sizeInBytes = Math.Min(sizeInBytes, stream.Length);

        byte[] buffer = new byte[sizeInBytes];

        while (true) {
            var bytesRead = await stream.ReadAsync(buffer);

            if (bytesRead <= 0) {
                break;
            }

            var chunkPath = $"{filename}-{chunkIndex}.chunk";

            await File.WriteAllBytesAsync(chunkPath, buffer);

            SmartConsole.LogInfo($"Successfully created chunk: {chunkPath}/{stream.Length / sizeInBytes}.");

            if (bytesRead < sizeInBytes) {
                break;
            }

            chunkIndex++;
        }
    }

    [Command("null-check", Description = "Calculates the null ratio for the specified file.")]
    public static void NullCheck([Option('p')] string path)
    {
        ShutdownManager.Subscribe();

        var stream = File.OpenRead(path);

        long index = 0;
        long nullBytes = 0;
        while (true) {
            index++;

            var @byte = stream.ReadByte();

            if (@byte == -1) {
                break;
            }

            if (@byte == 0) {
                nullBytes++;
            }

            if ((index + 1) % 150000000 == 0) {
                float currentNullRatio = (float)((double)index / nullBytes * 100);

                SmartConsole.LogInfo($"NullRatio: {currentNullRatio}%");
                SmartConsole.LogInfo($"Position: {(double)index / stream.Length * 100}%");
            }
        }

        // We cast the double to a float
        float nullRatio = (float)((double)nullBytes / stream.Length * 100);

        SmartConsole.LogInfo($"NullRatio: {nullRatio}%");
    }

    [Command("full-delete", Description = "Deletes the specified file by overwriting it " +
        "with random data for a specified amount of times then deleting it from disk.")]
    public static async Task FullDelete([Argument] string path, [Option('r')] int rounds = 1)
    {
        ShutdownManager.Subscribe();

        if (!File.Exists(path)) {
            SmartConsole.LogError("The specified file was not found.");
        }

        long length;
        using (var stream = File.OpenRead(path)) {
            length = stream.Length;
        }

        byte[] buffer = new byte[4096];
        for (int i = 0; i < rounds; i++) {
            SmartConsole.LogInfo($"Running round {i + 1}/{rounds}");
            Random.Shared.NextBytes(buffer);

            using var stream = File.OpenWrite(path);

            long position = 0;
            while (position < length) {
                await stream.WriteAsync(buffer.AsMemory()[..Math.Min((int)(length - position), buffer.Length)]);

                position += buffer.Length;
            }
        }

        SmartConsole.LogInfo("Deleting rewritten file...");

        File.Delete(path);
    }
}