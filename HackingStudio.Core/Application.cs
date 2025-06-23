using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HackingStudio.Core;

public static class Application
{
	public static Version Version { get; } = Version.Parse("1.0.0");

	public static string PrettyName => "HackingStudio";

	public static string UnixFolderName => ".HackingStudio";

	public static string DataPath
	{
		get {
			if (OperatingSystem.IsWindows()) {
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), UnixFolderName);
			}
			else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) {
				return $"/home/{Environment.UserName}/{UnixFolderName}";
			}

			throw new PlatformNotSupportedException($"HackingStudio does not supported the '{Environment.OSVersion.Platform}' platform.");
		}
	}

	public static string GetDataPath(params string[] relativePaths)
	{
		var fullPath = Path.Combine(DataPath, Path.Combine(relativePaths));

		var directory = Path.GetDirectoryName(fullPath)!;

		if (!Directory.Exists(directory)) {
			Directory.CreateDirectory(directory);
		}

		return fullPath;
	}

	public static string GetDataFolder(params string[] relativePaths)
	{
		var fullPath = Path.Combine(DataPath, Path.Combine(relativePaths));

		if (!Directory.Exists(fullPath)) {
			Directory.CreateDirectory(fullPath);
		}

		return fullPath;
	}

	public static void AssertSupportedPlatform()
	{
		if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS()) {
			SmartConsole.LogError($"PlatformNotSupportedException: Sorry HackingStudio is not supported on '{RuntimeInformation.OSDescription}'.");

			Process.GetCurrentProcess().Kill();
		}
	}

	public static async Task LoadThirdPartyLibrariesAsync()
	{
		var assemblyPaths = Directory.EnumerateFiles(GetDataFolder("dlls"), "*.dll", SearchOption.AllDirectories);

        foreach (var assemblyPath in assemblyPaths) {
			try {
				var assemblyContents = await File.ReadAllBytesAsync(assemblyPath);

				AppDomain.CurrentDomain.Load(assemblyContents);
			}
			catch (Exception ex) { 
				SmartConsole.LogError($"Failed to load {assemblyPath}: \r\n{ex.GetType()}: {ex.Message}");
			}
		}
	}
}