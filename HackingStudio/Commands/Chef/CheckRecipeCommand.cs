using System.Text;
using BosonWare.TerminalApp;
using Figgle;
using HackingStudio.Core.Chef;
using JetBrains.Annotations;
using Realtin.Xdsl;

namespace HackingStudio.Commands.Chef;

[UsedImplicitly]
[Command("check-recipe", Description = "Validates the provided recipe.")]
internal sealed class CheckRecipeCommand : ICommand, IChefCommand
{
    public async Task Execute(string recipe)
    {
        var recipeData = Encoding.UTF8.GetString(await CommandsHelpers.ReadIfFile(recipe));

        try {
            var recipeHandler = Recipe.Deserialize(recipeData);

            if (recipeHandler.Steps is not { } steps) {
                SmartConsole.LogError("This recipe does not have any step, sorry I can't cook your food hahaha.");

                return;
            }

            foreach (var definedStep in steps) {
                var name = definedStep.Name;

                if (StepFactory.TryGetStep(name, out var stepHandler)) {
                    SmartConsole.WriteLine(FiggleFonts.SlantSmall.Render(name), ConsoleColor.Green);

                    SmartConsole.WriteLine(stepHandler.Description, ConsoleColor.Gray);

                    continue;
                }

                SmartConsole.LogError($"Step '{definedStep.Name}' was not found.");
            }
        }
        catch (XdslException ex) {
            SmartConsole.LogError($"XdslError: {ex.Message}");
        }
    }
}
