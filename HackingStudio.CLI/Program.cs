using Cocona;
using Figgle;
using HackingStudio.CLI;
using HackingStudio.CLI.Forensics;
using HackingStudio.Core;

Application.AssertSupportedPlatform();

await Application.LoadThirdPartyLibrariesAsync();

if (args.Length == 0 || args.Contains("-h") || args.Contains("--help")) {
	var title = FiggleFonts.Slant.Render("Hacking Studio");

	SmartConsole.WriteLine(title, ConsoleColor.Green);
}

// Handle section aliases.
if (args.Length > 0 && args[0] == "fcs") {
    args[0] = "forensics";
}
else if (args.Length > 0 && args[0] == "proc") {
    args[0] = "process";
}

var builder = CoconaApp.CreateBuilder(args, configureOptions: options => {
	options.EnableConvertCommandNameToLowerCase = false;

	if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) {
		options.EnableShellCompletionSupport = true;
	}
});

var app = builder.Build();

app.AddCommands<ForensicsCommandsSection>();
app.AddCommands<BruteCommandsSection>();
app.AddCommands<ObfuscatorCommandsSection>();
app.AddCommands<NetworkCommandsSection>();
app.AddCommands<ProcessCommandsSection>();
app.AddCommands<ChefCommandsSection>();
app.AddCommands<PluginCommandsSection>();
app.AddCommands<FSCommandsSection>();
app.AddCommands<UtilityCommandsSection>();

ShutdownManager.Subscribe();

app.Run();