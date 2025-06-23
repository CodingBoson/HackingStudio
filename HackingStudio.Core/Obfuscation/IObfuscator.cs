namespace HackingStudio.Core.Obfuscation;

public interface IObfuscator
{
    string Obfuscate(string script, int layers, string options = "");
}