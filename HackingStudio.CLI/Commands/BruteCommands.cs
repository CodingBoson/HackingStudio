using System.Diagnostics;
using Cocona;
using HackingStudio.Core;

namespace HackingStudio.CLI;

[HasSubCommands(typeof(BruteCommands), "brute", Description = "Brute force commands 🐂.")]
public sealed class BruteCommandsSection; 

public sealed class BruteCommands
{
    [Command("find")]
    public static async Task Find(
        [Argument] string hash,
        [Option('a')] string algorithm,
        [Option('w')] string list,
        [Option('e')] string encoding = "base64",
        [Option('t')] int threads = -1)
    {
        var brute = new Brute(threads);

        SmartConsole.LogInfo($"""
            Algorithm: {algorithm.ToUpperInvariant()}
            HashEncoding: {encoding.ToUpperInvariant()}
            Threads: {brute.Threads}

            """);

        SmartConsole.LogInfo("Loading word list...");

        long startingMemory = GC.GetTotalMemory(forceFullCollection: false);

        var words = await Brute.LoadWordListAsync(list);

        SmartConsole.LogInfo("Starting brute force search...");
        Console.WriteLine();
        try {
            var watch = Stopwatch.StartNew();

            var result = brute.Find(hash, algorithm, encoding, words);

            var elapsedSeconds = watch.Elapsed.TotalSeconds;

            if (result is not null) {
                Console.WriteLine($"Found: {result} in {elapsedSeconds}s");
            }
            else {
                Console.WriteLine($"Not found. ({elapsedSeconds}s)");
            }
        }
        catch (Exception ex) {
            SmartConsole.LogError($"{ex.GetType().Name}: {ex.Message}");

            return;
        }

        long endingMemory = GC.GetTotalMemory(forceFullCollection: false);

        long bytesUsed = endingMemory - startingMemory;

        SmartConsole.LogInfo($"Memory Consumed: {IOUtility.PrettySize(bytesUsed)}");
    }
}