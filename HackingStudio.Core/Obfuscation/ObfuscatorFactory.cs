namespace HackingStudio.Core.Obfuscation;

public static class ObfuscatorFactory
{
    public static IObfuscator? Create(string lang)
    {
        return lang.ToLowerInvariant() switch {
            "powershell" => new PowerShellObfuscator(),
            _ => null
        };
    }
}
