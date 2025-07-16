namespace HackingStudio.CLI;

public static class ShutdownManager
{
	public static void Subscribe(Action? exited = null)
	{
		Console.CancelKeyPress += (sender, args) => {
			SmartConsole.Unlock(); // Unlock the console, if it's locked.
			SmartConsole.WriteLine("[HOST] Shutting down...", ConsoleColor.Cyan);
			SmartConsole.Lock();

			exited?.Invoke();

			Environment.Exit(0);
		};
	}
}