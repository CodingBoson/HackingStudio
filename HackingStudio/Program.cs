using BosonWare.TerminalApp;
using BosonWare.TerminalApp.BuiltIn;
using Figgle;
using HackingStudio;
using HackingStudio.Core;

Application.Initialize<IAssemblyMarker>();

await LibraryHelpers.LoadThirdPartyLibrariesAsync();

CommandRegistry.LoadCommands<IAssemblyMarker>(typeof(IBuiltInAssemblyMarker));

var app = await ConsoleApplication.CreateAsync("[[Crimson]HackingStudio[/]] [Violet]->[/] ");

app.TerminationMode = TerminationMode.IgnoreCtrlC;

app.AddCommand("force-exit", _ => {
    Environment.Exit(0);
}).AddDescription("Force exit");

Console.Clear();

SmartConsole.WriteLine(FiggleFonts.Slant.Render("HackingStudio"), ConsoleColor.Green);
SmartConsole.WriteLine("");

await app.RunAsync();
