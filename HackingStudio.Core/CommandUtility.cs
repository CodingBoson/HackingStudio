using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HackingStudio.Core;

public static class CommandUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static (string CommandName, string Args) Parse(string command)
	{
		command = command.Trim();

		int num = command.IndexOf(' ');

		if (num > 0) {
			var name = command[..num];
			var arguments = command[(num + 1)..];

			return (name, arguments);
		}

		return (command, "");
	}

	public static async Task RunAsync(string bin, string args)
	{
		var start = new ProcessStartInfo(bin, args) {
			RedirectStandardError = true,
			RedirectStandardOutput = true,
			RedirectStandardInput = true,
			CreateNoWindow = true,
		};

		var process = Process.Start(start)!;

		await process.WaitForExitAsync();

		Console.Write(process.StandardOutput.ReadToEnd());
		Console.Write(process.StandardError.ReadToEnd());

		Console.WriteLine();
	}
}