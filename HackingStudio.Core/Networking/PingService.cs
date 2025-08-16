using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HackingStudio.Core.Networking;

public enum PingProtocol
{
    ICMP,
    TCP
}

public static class PingService
{
    public static async Task Ping(string target,
        PingProtocol protocol,
        int timeout = 5000,
        double delay = 1,
        int count = 3,
        int bufferSize = 0,
        bool verboseOutput = false,
        CancellationToken cancellationToken = default)
    {
        if (count == -1) {
            while (!cancellationToken.IsCancellationRequested) {
                await HandlePing(target, protocol, timeout, bufferSize, verboseOutput, cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
            }
        }

        for (var i = 0; i < count; i++) {
            await HandlePing(target, protocol, timeout, bufferSize, verboseOutput, cancellationToken);


            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
        }
    }

    private static async Task HandlePing(string target,
        PingProtocol protocol,
        int timeout,
        int bufferSize,
        bool verboseOutput,
        CancellationToken cancellationToken)
    {
        switch (protocol) {
            case PingProtocol.ICMP:
                await ICMPPing(target, timeout, bufferSize, verboseOutput);

                break;

            case PingProtocol.TCP:
                await TCPPing(target, timeout, cancellationToken);

                break;

            default:
                SmartConsole.LogError($"The specified protocol '{protocol}' is not supported. (e.g ICMP (Default), TCP)");
                break;
        }
    }

    public static async Task SimplePing(string target,
        PingProtocol protocol,
        int timeout = 5000,
        int bufferSize = 0,
        bool verboseOutput = false,
        CancellationToken cancellationToken = default)
    {
        await HandlePing(target, protocol, timeout, bufferSize, verboseOutput, cancellationToken);
    }

    public static async Task ICMPPing(string target, int timeout, int bufferSize = 0, bool verboseOutput = false)
    {
        var ping = new Ping();

        try {
            PingReply replay;

            var startTime = DateTime.Now;
            if (bufferSize > 0) {
                var buffer = new byte[bufferSize];

                Random.Shared.NextBytes(buffer);

                replay = await ping.SendPingAsync(target, timeout, buffer);
            }
            else {
                replay = await ping.SendPingAsync(target, timeout);
            }

            if (replay.Status == IPStatus.Success) {
                var elapsed = DateTime.Now - startTime;

                SmartConsole.WriteLine($"Target {target} is up: {elapsed.TotalMilliseconds}ms", ConsoleColor.Magenta);
            }
            else {
                SmartConsole.WriteLine($"Target {target} is down: {replay.Status}.", ConsoleColor.Yellow);
            }

            if (verboseOutput) {
                SmartConsole.WriteLine($$"""
                                         {
                                             "IPAddress": "{{replay.Address}}",
                                             "Buffer": "{{Convert.ToBase64String(replay.Buffer)}}",
                                             "Options": {{replay.Options.ToJson()}},
                                             "RoundtripTime": "{{replay.RoundtripTime}}",
                                             "Status": "{{replay.Status}}"
                                         }
                                         """);
            }
        }
        catch (Exception ex) {
            SmartConsole.WriteLine($"Unexpected Error: {ex.Message}", ConsoleColor.Yellow);
        }

        ping.Dispose();
    }

    public static async Task TCPPing(string target, int timeout, CancellationToken cancellationToken)
    {
        var startTime = DateTime.Now;

        try {
            using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) {
                SendTimeout = timeout, ReceiveTimeout = timeout
            };

            //using var token = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout));

            //await socket.ConnectAsync(target, 80, token.Token);
            await socket.ConnectAsync(target, 80, cancellationToken);

            socket.Shutdown(SocketShutdown.Both);

            var elapsed = DateTime.Now - startTime;

            SmartConsole.WriteLine($"Target {target} is up: {elapsed.TotalMilliseconds}ms", ConsoleColor.Magenta);
        }
        catch (SocketException ex) {
            var elapsed = DateTime.Now - startTime;

            if (ex.SocketErrorCode == SocketError.ConnectionRefused) {
                SmartConsole.WriteLine($"Target {target} is up: {elapsed.TotalMilliseconds}ms", ConsoleColor.Magenta);
            }
        }
    }
}
