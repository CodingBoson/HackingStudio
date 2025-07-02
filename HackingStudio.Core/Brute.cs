using System.Security.Cryptography;
using System.Text;
using BosonWare.Encoding;

namespace HackingStudio.Core;

public sealed class Brute
{
    public int Threads { get; }

    public Brute(int threads)
    {
        if (threads <= 0)
            threads = Environment.ProcessorCount;

        Threads = threads;
    }

    public string? Find(string hash, string algorithm, string encoding, List<string> words)
    {
        using var cancellationSource = new CancellationTokenSource();

        ParallelOptions options = new() {
            MaxDegreeOfParallelism = Threads,
            CancellationToken = cancellationSource.Token
        };

        object @lock = new();
        string? match = null;

        try {
            Parallel.ForEach(words, options, (word) => {
                byte[] wordHash = Hash(algorithm, word);
                string encodedHash = Encode(wordHash, encoding);

                if (encodedHash == hash) {
                    lock (@lock) {
                        match = word;

                        // Break the loop for all threads.
                        cancellationSource.Cancel();
                    }
                }
            });
        }
        catch (OperationCanceledException) { }

        return match;
    }

    public static string Encode(byte[] hash, string encoding)
    {
        return encoding.ToLowerInvariant() switch {
            "base58" => Base58.EncodeData(hash),
            "base64" => Convert.ToBase64String(hash),
            "hex" => Convert.ToHexStringLower(hash),
            _ => throw new NotSupportedException($"Encoding {encoding} is not supported.")
        };
    }

    public static byte[] Hash(string algorithm, string word)
    {
        return algorithm.ToLowerInvariant() switch {
            "md5" => MD5.HashData(Encoding.UTF8.GetBytes(word)),
            "sha1" or "sha-1" => SHA1.HashData(Encoding.UTF8.GetBytes(word)),
            "sha256" or "sha-256" => SHA256.HashData(Encoding.UTF8.GetBytes(word)),
            "sha384" or "sha-384" => SHA384.HashData(Encoding.UTF8.GetBytes(word)),
            "sha512" or "sha-512" => SHA512.HashData(Encoding.UTF8.GetBytes(word)),
            _ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
        };
    }

    public static async Task<List<string>> LoadWordListAsync(
        string path,
        bool ignoreComments = true)
    {
        var lines = await File.ReadAllLinesAsync(path);

        var filteredWords = lines.Distinct();

        if (ignoreComments) {
            filteredWords = filteredWords
                .Where(x => !x.StartsWith("#!comment: ", StringComparison.OrdinalIgnoreCase));
        }

        return [.. filteredWords];
    }
}