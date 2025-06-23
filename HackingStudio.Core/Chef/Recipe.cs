using Realtin.Xdsl;
using System.Diagnostics.CodeAnalysis;

namespace HackingStudio.Core.Chef;

public sealed partial class Recipe(XdslDocument root)
{
    private readonly XdslDocument _root = root;

    public XdslElementCollection? Steps => _root.Root?.Children;

    // Pun intended.
    public bool TryCook(byte[] data, [NotNullWhen(true)] out byte[]? food, [NotNullWhen(false)] out string? error)
    {
        food = null;
        error = "";

        if (_root.Root?.Children is not XdslElementCollection steps) {
            error = $"This recipe does not have any step, sorry I can't cook your food.";

            return false;
        }

        var tempData = data;

        foreach (var definedStep in steps) { 
            if (StepFactory.TryGetStep(definedStep.Name, out var stepHandler)) {
                try {
                    tempData = stepHandler.Perform(tempData, definedStep);
                }
                catch (Exception ex) { 
                    error = $"{ex.GetType().Name}: {ex.Message}";

                    return false;
                }

                continue;
            }

            error = $"{error}{Environment.NewLine}Step '{definedStep.Name}' was not found.".TrimStart();
        }

        food = tempData;

        return true;
    }

    public static Recipe Deserialize(string data)
    {
        var root = XdslDocument.Create(data);

        var recipe = new Recipe(root);

        return recipe;
    }
}