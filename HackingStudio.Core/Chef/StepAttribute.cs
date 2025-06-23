namespace HackingStudio.Core.Chef;

[AttributeUsage(AttributeTargets.Class)]
public sealed class StepAttribute(string name) : Attribute
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
}
