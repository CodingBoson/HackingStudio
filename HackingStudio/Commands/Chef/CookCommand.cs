using System.Text;
using BosonWare.TerminalApp;
using CommandLine;
using HackingStudio.Core.Chef;
using JetBrains.Annotations;

namespace HackingStudio.Commands.Chef;

[Group("chef", Description = "Prepare and Cook cyber recipes.")]
internal interface IChefCommand;

[UsedImplicitly]
internal sealed class CookOptions
{
    [UsedImplicitly]
    [Option('d', Required = true, HelpText = "The data file to use.")]
    public required string Data { get; set; }

    [UsedImplicitly]
    [Option('r', Required = true, HelpText = "The relative path to a file to use.")]
    public required string Recipe { get; set; }

    [UsedImplicitly]
    [Option('o', Required = true, HelpText = "The output path", Default = "stdout")]
    public required string Output { get; set; } = "stdout";
}

[UsedImplicitly]
[Command("cook", Description = "Runs the provided recipe.")]
internal sealed class CookCommand : Command<CookOptions>, IChefCommand
{
    public override async Task Execute(CookOptions options)
    {
        var dataBytes = await CommandsHelpers.ReadIfFile(options.Data);
        var recipeData = await File.ReadAllTextAsync(options.Recipe);

        var recipeHandler = Recipe.Deserialize(recipeData);

        if (recipeHandler.TryCook(dataBytes, out var cookedFood, out var error)) {
            if (options.Output.Equals("stdout", StringComparison.InvariantCultureIgnoreCase)) {
                var utf8Food = Encoding.UTF8.GetString(cookedFood);

                Console.WriteLine(utf8Food);

                return;
            }

            await File.WriteAllBytesAsync(options.Output, cookedFood);

            return;
        }

        SmartConsole.LogError(error);
    }
}
