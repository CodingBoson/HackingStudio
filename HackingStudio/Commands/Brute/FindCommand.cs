using System.Diagnostics;
using BosonWare.TerminalApp;
using CommandLine;
using HackingStudio.Core;
using JetBrains.Annotations;

namespace HackingStudio.Commands.Brute;

[UsedImplicitly]
file sealed record FindOptions
{
    [Value(
        0, MetaName = "hash",
        Required = true,
        HelpText = "The hash you want to search for.")]
    public required string Hash { get; init; }

    [Option(
        'a',
        "algorithm",
        Required = true,
        HelpText = "Algorithm to use for brute force operations.")]
    public required string Algorithm { get; init; }

    [Option(
        'w',
        "wordList",
        Required = true,
        HelpText = "Word list file containing the non-hashed words.")]
    public required string WordList { get; init; }

    [Option(
        'e',
        "encoding",
        Required = false,
        Default = "base64",
        HelpText = "The encoding to be used for encoding.")]
    public string Encoding { get; } = "base64";

    [Option(
        't',
        "threads",
        Required = false,
        Default = -1,
        HelpText = "Number of threads to use")]
    public int Threads { get; } = -1;

    public void Deconstruct(
        out string hash,
        out string algorithm,
        out string wordList,
        out string encoding,
        out int threads)
    {
        hash = Hash;
        algorithm = Algorithm;
        wordList = WordList;
        encoding = Encoding;
        threads = Threads;
    }
}

[UsedImplicitly]
[Command("find", Description = "Finds the non-hashed word for the specified hash.")]
[Group("brute", Description = "Brute force commands 🐂.")]
file sealed class FindCommand : Command<FindOptions>
{
    public override async Task Execute(FindOptions options)
    {
        var (hash, algorithm, wordList, encoding, threads) = options;

        var brute = new Core.Brute(threads);

        SmartConsole.LogInfo($"""
                              Algorithm: {algorithm.ToUpperInvariant()}
                              HashEncoding: {encoding.ToUpperInvariant()}
                              Threads: {brute.Threads}

                              """);

        SmartConsole.LogInfo("Loading word list...");

        var startingMemory = GC.GetTotalMemory(forceFullCollection: false);

        var words = await Core.Brute.LoadWordListAsync(wordList);

        SmartConsole.LogInfo("Starting brute force search...");
        Console.WriteLine();
        try {
            var watch = Stopwatch.StartNew();

            var result = brute.Find(hash, algorithm, encoding, words);

            var elapsedSeconds = watch.Elapsed.TotalSeconds;

            Console.WriteLine(result is not null
                ? $"Found: {result} in {elapsedSeconds}s"
                : $"Not found. ({elapsedSeconds}s)");
        }
        catch (Exception ex) {
            SmartConsole.LogError($"{ex.GetType().Name}: {ex.Message}");

            return;
        }

        var endingMemory = GC.GetTotalMemory(forceFullCollection: false);

        var bytesUsed = endingMemory - startingMemory;

        SmartConsole.LogInfo($"Memory Consumed: {IOUtility.PrettySize(bytesUsed)}");
    }
}
