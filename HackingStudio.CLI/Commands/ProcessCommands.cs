using System.Diagnostics;
using Cocona;
using HackingStudio.Core;
using HackingStudio.Core.Processes;

namespace HackingStudio.CLI;

[HasSubCommands(typeof(ProcessCommands), "process", Description = "Process commands. (Alias: proc)")]
public sealed class ProcessCommandsSection;

public sealed class ProcessCommands
{
    [Command("find")]
    public static void FindProcess([Argument] string name, [Option] bool exactMatch = false)
    {
        var processes = ProcessAnalyzer.FindProcesses(name, exactMatch);

        var longestNameLength = processes.Select(x => GetPath(x).Trim().Length).Max();

        foreach (var process in processes) {
            var path = GetPath(process).Trim();
            var padding = "     ";

            if (path.Length == longestNameLength) {
                Console.WriteLine($"{path}{padding}Id: {process.Id}");

                continue;
            }

            padding += new string(' ', longestNameLength - path.Length);

            Console.WriteLine($"{path}{padding}Id: {process.Id}");
        }
    }

    [Command("attach")]
    public static async Task Attach([Argument] int processId,
        [Option('i')] bool interactive = false)
    {
        try {
            await ProcessAnalyzer.AttachTo(processId, interactive);
        }
        catch (Exception ex) {
            SmartConsole.LogError($"{ex.GetType().Name}: {ex.Message}");
        }
    }

    [Command("start")]
    public static async Task Start([Argument] string process,
        [Option('i')] bool interactive = false)
    {
        try {
            await ProcessAnalyzer.Start(process, interactive);
        }
        catch (Exception ex) {
            SmartConsole.LogError($"{ex.GetType().Name}: {ex.Message}");
        }
    }

    [Command("kill")]
    public static void Kill([Argument] string process)
    {
        // We check to see if the provided 'process' is a 
        // valid integer, if not, 
        // we provide the user with the best processes by names.
        if (!int.TryParse(process, out int id)) {
            FindProcess(process);

            var userId = SmartConsole.ReadLine("Enter the Id of the process you wish to kill: ");

            id = int.Parse(userId);
        }

        try {
            var proc = Process.GetProcessById(id);

            proc.Kill(entireProcessTree: true);
        }
        catch (Exception ex) {
            SmartConsole.LogError($"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static string GetPath(Process process)
        => process.MainModule?.FileName ?? process.ProcessName;
}