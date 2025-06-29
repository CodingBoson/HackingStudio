using Cocona;
using HackingStudio.Core;

namespace HackingStudio.CLI;

[HasSubCommands(typeof(UtilityCommands), "util", Description = "Utility commands for HackingStudio.")]
public sealed class UtilityCommandsSection { }

public sealed class UtilityCommands
{
	//[Command("DATA-PATH", Description = "Prints the data path for HackingStudio.", Aliases = ["data"])]
	[Command("GetDataPath", Description = "Prints the data path for HackingStudio. (Alias: data)", Aliases = ["data"])]
	public static void GetDataPath() => Console.WriteLine(Application.DataPath);

	[Command("GetUpdate", Description = "Checks for an newer version of HackingStudio. (Alias: update)", Aliases = ["update"])]
	public static async Task GetUpdate()
	{
		const string versionUrl = "https://hacking-studio.secure-pages.uk/new-version";

		try {
			using var client = new HttpClient();

			var response = await client.GetAsync(versionUrl);

			var newVersionText = await response.Content.ReadAsStringAsync();

			var newVersion = new Version(newVersionText);

			if (newVersion > Application.Version) {
				SmartConsole.LogInfo($"Found a new version: {newVersionText}");

				return;
			}

			SmartConsole.LogInfo("Your up to date.");
		}
		catch (HttpRequestException ex) {
			SmartConsole.LogError($"HttpRequestError: {ex.Message}");
			SmartConsole.LogWarning($"Please check your internet connection or check if {versionUrl} is up.");
		}
		catch (Exception ex) {
			SmartConsole.LogError($"UnexpectedError: {ex.Message}");
		}
	}
}