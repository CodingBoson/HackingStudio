using System.Runtime.CompilerServices;

namespace HackingStudio.Core;

/// <summary>
/// Provides thread-safe console operations.
/// </summary>
public static class SmartConsole
{
	private sealed class ThreadedConsole
	{
		private bool _isLocked = false;

		/// <summary>
		/// Locks the console to prevent writing.
		/// </summary>
		public void Lock() => _isLocked = true;

		/// <summary>
		/// Unlocks the console to allow writing.
		/// </summary>
		public void Unlock() => _isLocked = false;

		/// <summary>
		/// Writes a line to the console with the specified color.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="color">The color of the message.</param>
		public void WriteLine(object? message, ConsoleColor color = ConsoleColor.White)
		{
			if (_isLocked)
				return;

			var previousColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(message?.ToString() ?? "NULL");
			Console.ForegroundColor = previousColor;
		}

		/// <summary>
		/// Writes a message to the console with the specified color.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="color">The color of the message.</param>
		public void Write(object? message, ConsoleColor color = ConsoleColor.White)
		{
			if (_isLocked)
				return;

			var previousColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(message?.ToString() ?? "NULL");
			Console.ForegroundColor = previousColor;
		}

		/// <summary>
		/// Reads a line from the console with a prompt and the specified color.
		/// </summary>
		/// <param name="prompt">The prompt to display.</param>
		/// <param name="color">The color of the prompt.</param>
		/// <returns>The input from the console.</returns>
		public string ReadLine(string prompt, ConsoleColor color = ConsoleColor.White)
		{
			if (_isLocked)
				return string.Empty;

			Write(prompt, color);
			return Console.ReadLine() ?? string.Empty;
		}
	}

	private static readonly ThreadedConsole _consoleInstance = new();

	/// <summary>
	/// Locks the console to prevent writing.
	/// </summary>
	public static void Lock()
	{
		lock (_consoleInstance) {
			_consoleInstance.Lock();
		}
	}

	/// <summary>
	/// Unlocks the console to allow writing.
	/// </summary>
	public static void Unlock()
	{
		lock (_consoleInstance) {
			_consoleInstance.Unlock();
		}
	}

	/// <summary>
	/// Writes a line to the console with the specified color.
	/// </summary>
	/// <param name="message">The message to write.</param>
	/// <param name="color">The color of the message.</param>
	public static void WriteLine(object? message, ConsoleColor color = ConsoleColor.White)
	{
		lock (_consoleInstance) {
			_consoleInstance.WriteLine(message, color);
		}
	}

	/// <summary>
	/// Writes a message to the console with the specified color.
	/// </summary>
	/// <param name="message">The message to write.</param>
	/// <param name="color">The color of the message.</param>
	public static void Write(object? message, ConsoleColor color = ConsoleColor.White)
	{
		lock (_consoleInstance) {
			_consoleInstance.Write(message, color);
		}
	}

	/// <summary>
	/// Reads a line from the console with a prompt and the specified color.
	/// </summary>
	/// <param name="prompt">The prompt to display.</param>
	/// <param name="color">The color of the prompt.</param>
	/// <returns>The input from the console.</returns>
	public static string ReadLine(string prompt, ConsoleColor color = ConsoleColor.White)
	{
		lock (_consoleInstance) {
			return _consoleInstance.ReadLine(prompt, color);
		}
	}

	/// <summary>
	/// Logs an info message to the console.
	/// </summary>
	/// <param name="message">The message to log.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogInfo(object? message) => Log("INFO", message, ConsoleColor.Green);

	/// <summary>
	/// Logs a warning message to the console.
	/// </summary>
	/// <param name="message">The message to log.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarning(object? message) => Log("WARNING", message, ConsoleColor.Yellow);

	/// <summary>
	/// Logs an error message to the console.
	/// </summary>
	/// <param name="message">The message to log.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogError(object? message) => Log("ERROR", message, ConsoleColor.Red);

	/// <summary>
	/// Logs a message to the console with the specified type and color.
	/// </summary>
	/// <param name="type">The type of the message.</param>
	/// <param name="message">The message to log.</param>
	/// <param name="color">The color of the message.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Log(string type, object? message, ConsoleColor color)
	{
		var formatted = (message?.ToString() ?? "NULL")
			.ReplaceLineEndings(Environment.NewLine + new string(' ', type.Length + 3));

		Write($"[{type}] ", color);
		WriteLine(formatted);
	}
}