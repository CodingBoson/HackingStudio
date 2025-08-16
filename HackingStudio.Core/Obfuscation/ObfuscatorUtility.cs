using BosonWare.Encoding;

namespace HackingStudio.Core.Obfuscation;

public static class ObfuscatorUtility
{
    public static string VariableName(int min = 10, int max = 30)
    {
        var buffer = new byte[Random.Shared.Next(min, max + 1)];

        Random.Shared.NextBytes(buffer);

        return Base58.EncodeData(buffer);
    }
}
