using System.Diagnostics;

namespace HackingStudio.Core;

public sealed class PluginEnvironment(Plugin plugin) : IDisposable
{
    public const string EnvironmentDirectory = ".hs-env";

    private readonly Plugin _plugin = plugin;

    public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;

    public void Dispose()
    {
        if (Directory.Exists(EnvironmentDirectory)) {
            Directory.Delete(EnvironmentDirectory, true);
        }
    }

    public async Task ExecuteAsync(string arguments)
    {
        Directory.SetCurrentDirectory(PluginManager.GetPluginRoot(_plugin));

        SetupUnixFileModes(_plugin);

        // Set up environment variables.
        Directory.CreateDirectory(EnvironmentDirectory);

        File.WriteAllText(Path.Combine(EnvironmentDirectory, "path"), WorkingDirectory);

        var (commandName, args) = CommandUtility.Parse(_plugin.Command);

        var processStart = new ProcessStartInfo(commandName, $"{args} {arguments}".Trim()) {
            CreateNoWindow = false
        };

        processStart.Environment.Add("HSPATH", WorkingDirectory);

        using var process = Process.Start(processStart)!;

        await process.WaitForExitAsync();
    }

    private static void SetupUnixFileModes(Plugin plugin)
    {
        if (plugin.UnixFileModes.Count != 0 && (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())) {
            foreach (var mode in plugin.UnixFileModes) {
                var parts = mode.Split(":");

                if (parts.Length != 2) {
                    SmartConsole.LogInfo($"The specified unix file mode '{mode}' is not in the correct format e.g. (FileName:Mode).");

                    continue;
                }

                var filename = parts[0];
                var unixMode = parts[1];

                try {
                    var fileMode = unixMode.StartsWith('+')
                        ? File.GetUnixFileMode(filename) | Enum.Parse<UnixFileMode>(unixMode.AsSpan(1))
                        : Enum.Parse<UnixFileMode>(unixMode);

                    File.SetUnixFileMode(filename, fileMode);
                }
                catch (Exception ex) {
                    SmartConsole.LogError(ex.Message);
                }
            }
        }
    }
}
