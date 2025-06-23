using Realtin.Xdsl;
using Realtin.Xdsl.Serialization;

namespace HackingStudio.Core;

public static class PluginManager
{
	private const string CacheLabel = "HackingStudio.Core.PluginManager";

	public static string PluginsPath => Application.GetDataFolder("plugins");

	public static string GetPluginRoot(Plugin plugin)
	{
		var rootDirectory = Application.GetDataFolder(PluginsPath, plugin.Name.ToLowerInvariant());

		return rootDirectory;
	}

	public static async Task<bool> IsInstalledAsync(string name) => (await GetPluginAsync(name)) is not null;

	public static async Task InstallAsync(Plugin plugin, string source)
	{
		var rootDirectory = Application.GetDataFolder(PluginsPath, plugin.Name.ToLowerInvariant());

		if (Directory.Exists(source)) {
			var sourceFiles = Directory.GetFiles(source, "", SearchOption.AllDirectories);

			foreach (var file in sourceFiles) {
				var destinationPath = Path.Combine(rootDirectory, file);
				var dir = Path.GetDirectoryName(destinationPath);

				if (dir is not null && !Directory.Exists(dir)) {
					Directory.CreateDirectory(dir);
				}

				await File.WriteAllBytesAsync(destinationPath, await File.ReadAllBytesAsync(file));
			}
		}
		else if (File.Exists(source)) {
			var filePath = Path.Combine(rootDirectory, source);

			await File.WriteAllBytesAsync(filePath, await File.ReadAllBytesAsync(source));
		}

		await SetPluginAsync(plugin);
	}

	public static async Task<bool> UninstallAsync(string name)
	{
		if ((await GetPluginAsync(name)) is not Plugin plugin) return false;

		var path = Application.GetDataFolder(PluginsPath, plugin.Name.ToLowerInvariant());

		Directory.Delete(path, recursive: true);

		return true;
	}

	public static async Task RenameAsync(string pluginName, string newName)
	{
		if ((await GetPluginAsync(pluginName)) is not Plugin plugin) {
			return;
		}

		var oldPath = Application.GetDataPath(PluginsPath, pluginName.ToLowerInvariant());
		var newPath = Application.GetDataPath(PluginsPath, newName.ToLowerInvariant());

		Directory.Move(oldPath, newPath);

		plugin.Name = newName;

		await SetPluginAsync(plugin);
	}

	public static async Task<Plugin?> GetPluginAsync(string name)
	{
		var plugins = await GetPluginsAsync();

		return plugins.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
	}

	public static async Task SetPluginAsync(Plugin plugin)
	{
		Cache.Remove(CacheLabel);

		var path = Application.GetDataPath(PluginsPath, plugin.Name.ToLowerInvariant(), "plugin.conf");

		var document = XdslSerializer.Serialize(plugin);

		document.DocType = XdslDocumentType.Create("HackingStudio:Plugin");
		document.Version = XdslVersion.OnePoint0;

		await document.SaveAsync(path);
	}

	public static async Task<List<Plugin>> GetPluginsAsync()
	{
		return await Cache.GetValueAsync(CacheLabel, Load, TimeSpan.FromSeconds(5));

		static async Task<List<Plugin>> Load()
		{
			var configFiles = Directory.GetFiles(PluginsPath, "plugin.conf", SearchOption.AllDirectories);

			var plugins = new List<Plugin>();

			foreach(var path in configFiles) {
				var xdslData = await File.ReadAllTextAsync(path);

				var plugin = XdslSerializer.Deserialize<Plugin>(xdslData);

				if (plugin is null) {
					SmartConsole.LogError($"{path} is empty.");

					continue;
				}

				plugins.Add(plugin);
			}

			return plugins;
		}
	}

	public static async Task<List<Plugin>> SearchAsync(string term)
	{
		var plugins = await GetPluginsAsync();

		return plugins.Where(x => x.Name.Contains(term, StringComparison.InvariantCultureIgnoreCase)).
					   ToList();
	}
}