using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace HackingStudio.Core.Chef;

public static class StepFactory
{
    private static readonly Dictionary<string, Recipe.Step> _steps = new(new EqualityComparer());

    static StepFactory()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies.Where(x => Attribute.IsDefined(x, typeof(AssemblyHasStepsAttribute)))) {
            var types = assembly.GetExportedTypes();

            foreach (var type in types.Where(x => typeof(Recipe.Step).IsAssignableFrom(x) && !x.IsAbstract)) {
                if (type.GetCustomAttribute<StepAttribute>() is StepAttribute stepAttribute) {
                    var name = stepAttribute.Name;

                    if (_steps.ContainsKey(name)) {
                        SmartConsole.LogError($"Step '{name}' is already defined.");

                        continue;
                    }

                    try {
                        var step = (Recipe.Step)Activator.CreateInstance(type)!;

                        _steps.Add(name, step);
                    }
                    catch (Exception ex) {
                        SmartConsole.LogError($"Error While Instantiating Recipe.Step: {ex.Message}");
                    }
                }
            }
        }
    }

    public static IDictionary<string, Recipe.Step> Steps => _steps;

    public static bool TryGetStep(string name, [NotNullWhen(true)] out Recipe.Step? step)
    {
        return _steps.TryGetValue(name, out step);
    }

    private sealed class EqualityComparer : IEqualityComparer<string>
    {
        bool IEqualityComparer<string>.Equals(string? x, string? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return true;

            return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
        }

        int IEqualityComparer<string>.GetHashCode(string obj)
        {
            return obj.GetHashCode(StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
