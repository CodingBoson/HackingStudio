using System.Text;

namespace HackingStudio.Core.Obfuscation;

public class PowerShellObfuscator : IObfuscator
{
    public string Obfuscate(string script, int layers, string options = "")
    {
        var builder = new StringBuilder();

        var payloadVariable = ObfuscatorUtility.VariableName();

        var payload = script;

        for (int i = 0; i < layers; i++) {
            payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
        }

        builder.AppendLine($"${payloadVariable}=\"{payload}\"");

        var layersVariable = AssignNumber(builder, layers);
        var layerVariable = ObfuscatorUtility.VariableName();
        var layersAsIntVar = ObfuscatorUtility.VariableName();
        var layerIncrement = AssignNumber(builder, 1);

        var vars = Random.Shared.Next(15, 45);
        for (int i = 0; i < vars; i++) {
            var data = new byte[Random.Shared.Next(8, 64)];

            Random.Shared.NextBytes(data);

            builder.AppendLine($"${ObfuscatorUtility.VariableName()} = \"{Convert.ToBase64String(data)}\"");
        }

        if (options.Contains("--useLoop")) {
            var expressionVariable = ObfuscatorUtility.VariableName();

            builder.AppendLine($$"""
             ${{expressionVariable}}=${{payloadVariable}}
             ${{layerVariable}} = 1
            
             ${{layersAsIntVar}} = {{ReadNumber(layersVariable)}}
             while (${{layerVariable}} -le ${{layersAsIntVar}}) {
                 ${{expressionVariable}} = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(${{expressionVariable}}))
                 
                 ${{layerVariable}} += {{ReadNumber(layerIncrement)}}
             }
            
             iex ${{expressionVariable}}
             """);
        }
        else {
            var variable = payloadVariable;
            for (int i = 0; i < layers; i++) {
                var newVariable = ObfuscatorUtility.VariableName();

                builder.AppendLine($"""
                ${newVariable} = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(${variable}))
                """);

                variable = newVariable;
            }

            builder.AppendLine();
            builder.AppendLine($"iex ${variable}");
        }

        return builder.ToString();
    }

    protected static string AssignNumber(StringBuilder builder, float number)
    {
        var (line, variable) = AssignNumber(number);

        builder.AppendLine(line);

        return variable;
    }

    protected static (string Line, string VariableName) AssignNumber(float number)
    {
        var variableName = ObfuscatorUtility.VariableName();

        var line = $"${variableName}=\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(number.ToString()))}\"";

        return (Line: line, VariableName: variableName);
    }

    protected static string ReadNumber(string variable)
    {
        return $"[System.Int32]::Parse([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(${variable})))";
    }
}