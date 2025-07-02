using Cocona;
using HackingStudio.Core;
using HackingStudio.Core.Obfuscation;

namespace HackingStudio.CLI;


[HasSubCommands(typeof(ObfuscatorCommands), "obfuscator", Description = "Obfuscator commands.")]
public sealed class ObfuscatorCommandsSection;

public sealed class ObfuscatorCommands
{
    [Command("perform")]
    public static async Task Perform(
        [Argument] string scriptPath,
        [Option('n')] string language,
        [Option('o')] string output = "stdout",
        [Option('l')] int layers = 5,
        [Option('a')] string options = "")
    {
        IObfuscator? obfuscator = ObfuscatorFactory.Create(language);

        if (obfuscator is null) {
            SmartConsole.LogError($"Language '{language}' has no registered obfuscator.");

            return;
        }

        var script = await File.ReadAllTextAsync(scriptPath);

        var obfuscated = obfuscator.Obfuscate(script, layers, options);

        if (output == "stdout") {
            Console.WriteLine(obfuscated);
        }
        else {
            await File.WriteAllTextAsync(output, obfuscated);
        }
    }
}