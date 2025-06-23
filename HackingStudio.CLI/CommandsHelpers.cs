using System.Text;

namespace HackingStudio.CLI;

internal static class CommandsHelpers
{

    public static async Task<byte[]> GetSmartData(string data)
    {
        if (File.Exists(data)) {
            return await File.ReadAllBytesAsync(data);
        }

        return Encoding.UTF8.GetBytes(data);
    }
}