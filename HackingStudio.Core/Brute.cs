using BosonWare.Cryptography;
using BosonWare.Encoding;
using System.Security.Cryptography;
using System.Text;

namespace HackingStudio.Core;

public sealed class Brute
{
    public int Threads { get; }

    public Brute(int threads)
    {
        if (threads < 0) threads = Environment.ProcessorCount;

        Threads = threads;
    }

    public static async Task<List<string>> LoadWordListAsync(string path, bool ignoreComments = true)
    {
        var lines = await File.ReadAllLinesAsync(path);

        var filteredWords = lines.Distinct();

        if (ignoreComments) {
            filteredWords = filteredWords.SkipWhile(x => x.StartsWith("#!comment: ", StringComparison.OrdinalIgnoreCase));
        }

        return filteredWords.ToList();
    }

    public string? Find(string hash, string algorithm, string encoding, List<string> words)
    {
        ParallelOptions options = new ParallelOptions {
            MaxDegreeOfParallelism = Threads
        };

        object @lock = new();
        string? foundWord = null;

        Parallel.ForEach(words, options, (word) => {
            byte[] wordHash = Hash(algorithm, word);
            string encodedHash = Encode(wordHash, encoding);

            if (encodedHash == hash) {
                lock (@lock) {
                    foundWord = word;
                }
            }
        });

        return foundWord;
    }

    public static string Encode(byte[] hash, string encoding)
    {
        return encoding.ToLowerInvariant() switch {
            "base58" => Base58.EncodeData(hash),
            "base64" => Convert.ToBase64String(hash),
            "hex" => BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(),
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
}