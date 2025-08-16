using System.Text;

namespace HackingStudio.Commands;

internal static class CommandsHelpers
{
    /// <summary>
    ///     Reads the specified <paramref name="data" /> if it's a valid file path, otherwise returns it as UTF-8 encoded
    ///     bytes.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static async Task<byte[]> ReadIfFile(string data)
    {
        if (File.Exists(data)) {
            return await File.ReadAllBytesAsync(data);
        }

        return Encoding.UTF8.GetBytes(data);
    }
}
