namespace HackingStudio.Core;

public static class LibraryHelpers
{
	public static async Task LoadThirdPartyLibrariesAsync()
	{
		var assemblyPaths = Directory.EnumerateFiles(Application.GetDirectory("dlls"), "*.dll", SearchOption.AllDirectories);

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