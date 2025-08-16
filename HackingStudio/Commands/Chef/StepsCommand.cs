using BosonWare.TerminalApp;
using Figgle;
using HackingStudio.Core.Chef;
using JetBrains.Annotations;

namespace HackingStudio.Commands.Chef;

[UsedImplicitly]
[Command("steps", Description = "Gets available steps for cyber recipes.")]
internal sealed class StepsCommand : ICommand, IChefCommand
{
    public Task Execute(string arguments)
    {
        foreach (var (name, stepHandler) in StepFactory.Steps) {
            SmartConsole.WriteLine(FiggleFonts.SlantSmall.Render(name), ConsoleColor.Green);

            SmartConsole.WriteLine(stepHandler.Description, ConsoleColor.Gray);
        }

        Console.WriteLine();

        SmartConsole.WriteLine("[Bright]Simple Recipe A:[/]");
        SmartConsole.WriteLine("""
                               <Recipe>
                               	<AesEncrypt key="Your_Password"/>
                               	<Base64Encode />
                               </Recipe>
                               """, ConsoleColor.DarkYellow);

        SmartConsole.WriteLine("[Bright]Simple Recipe B:[/]");
        SmartConsole.WriteLine("""
                               <Recipe>
                               	<Base64Decode />
                               	<AesDecrypt key="Your_Password"/>
                               </Recipe>
                               """, ConsoleColor.DarkYellow);

        return Task.CompletedTask;
    }
}
