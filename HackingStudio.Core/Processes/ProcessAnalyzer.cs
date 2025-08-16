using System.Diagnostics;

namespace HackingStudio.Core.Processes;

public static class ProcessAnalyzer
{
    public static List<Process> FindProcesses(string name, bool exactMatch)
    {
        List<Process> processes = [];

        if (exactMatch) {
            processes.AddRange(Process.GetProcessesByName(name));
        }
        else {
            processes.AddRange(Process.GetProcesses().Where(x => x.ProcessName.Contains(name, StringComparison.InvariantCultureIgnoreCase)));
        }

        return processes;
    }

    public static async Task AttachTo(int processId, bool interactive)
    {
        var process = Process.GetProcessById(processId);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.OutputDataReceived += (sender, data) => {
            if (!string.IsNullOrEmpty(data.Data)) {
                Console.WriteLine(data.Data.Trim());
            }
        };

        process.ErrorDataReceived += (sender, data) => {
            if (!string.IsNullOrEmpty(data.Data)) {
                SmartConsole.WriteLine(data.Data.Trim(), ConsoleColor.DarkRed);
            }
        };

        if (interactive) {
            CreateInteractiveConsoleThread(process);
        }

        await process.WaitForExitAsync();
    }

    public static async Task Start(string fileName, bool interactive)
    {
        var startInfo = new ProcessStartInfo(fileName) {
            RedirectStandardError = true, RedirectStandardOutput = true, RedirectStandardInput = interactive
            //CreateNoWindow = true,
        };

        var process = Process.Start(startInfo)!;

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.OutputDataReceived += (sender, data) => {
            if (!string.IsNullOrEmpty(data.Data)) {
                Console.WriteLine(data.Data.Trim());
            }
        };

        process.ErrorDataReceived += (sender, data) => {
            if (!string.IsNullOrEmpty(data.Data)) {
                SmartConsole.WriteLine(data.Data.Trim(), ConsoleColor.DarkRed);
            }
        };

        if (interactive) {
            CreateInteractiveConsoleThread(process);
        }

        await process.WaitForExitAsync();
    }

    private static void CreateInteractiveConsoleThread(Process process)
    {
        new Thread(async () => {
            var i = Console.Read();

            while (i > 0) {
                await process.StandardInput.WriteAsync((char)i);

                i = Console.Read();
            }
        }).Start();
    }
}
