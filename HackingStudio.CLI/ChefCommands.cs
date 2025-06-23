using Cocona;
using Figgle;
using HackingStudio.Core;
using HackingStudio.Core.Chef;
using Realtin.Xdsl;
using System.Text;

namespace HackingStudio.CLI;

[HasSubCommands(typeof(ChefCommands), "chef", Description = "Prepare and Cook cyber recipes.")]
public sealed class ChefCommandsSection { }

public sealed class ChefCommands
{
    [Command("cook")]
    public static async Task Cook(
        [Option('d')] string data, 
        [Option('r')] string recipe, 
        [Option('o')] string output = "stdout")
    {
        var dataBytes = await CommandsHelpers.GetSmartData(data);
        var recipeData = await File.ReadAllTextAsync(recipe);

        var recipeHandler = Recipe.Deserialize(recipeData);

        if (recipeHandler.TryCook(dataBytes, out var cookedFood, out var error)) {
            if (output.Equals("stdout", StringComparison.InvariantCultureIgnoreCase)) {
                var utf8Food = Encoding.UTF8.GetString(cookedFood);

                Console.WriteLine(utf8Food);

                return;
            }

            await File.WriteAllBytesAsync(output, cookedFood);

            return;
        }

        SmartConsole.LogError(error);
    }

    [Command("steps", Description = "Gets available steps for cyber recipes.")]
    public static void Steps(bool noAsciiArt = false)
    {
        foreach (var (name, stepHandler) in StepFactory.Steps) {
            if (noAsciiArt) {
                SmartConsole.WriteLine($"\r\n{name}:");
            }
            else {
                SmartConsole.WriteLine(FiggleFonts.SlantSmall.Render(name), ConsoleColor.Green);
            }

            SmartConsole.WriteLine(stepHandler.Description, ConsoleColor.Gray);
        }

        SmartConsole.WriteLine("\r\nSimple Recipe A:");
        SmartConsole.WriteLine("""
            <Recipe>
            	<AesEncrypt key="Your_Password"/>
            	<Base64Encode />
            </Recipe>
            """, ConsoleColor.DarkYellow);

        SmartConsole.WriteLine("Simple Recipe B:");
        SmartConsole.WriteLine("""
            <Recipe>
            	<Base64Decode />
            	<AesDecrypt key="Your_Password"/>
            </Recipe>
            """, ConsoleColor.DarkYellow);
    }

    [Command("check-recipe")]
    public static async Task CheckRecipe([Argument] string recipe, bool noAsciiArt = false)
    {
        var recipeData = Encoding.UTF8.GetString(await CommandsHelpers.GetSmartData(recipe));

        try {
            var recipeHandler = Recipe.Deserialize(recipeData);

            if (recipeHandler.Steps is not XdslElementCollection steps) {
                SmartConsole.LogError("This recipe does not have any step, sorry I can't cook your food hahaha.");

                return;
            } 

            foreach (var definedStep in steps) {
                var name = definedStep.Name;

                if (StepFactory.TryGetStep(name, out var stepHandler)) {

                    if (noAsciiArt) {
                        SmartConsole.WriteLine($"\r\n{name}:");
                    }
                    else {
                        SmartConsole.WriteLine(FiggleFonts.SlantSmall.Render(name), ConsoleColor.Green);
                    }

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