using Cocona;
using HackingStudio.Core;
using HackingStudio.Core.Networking;

namespace HackingStudio.CLI;

[HasSubCommands(typeof(NetworkCommands), "net", Description = "Network commands.")]
public sealed class NetworkCommandsSection { }

public sealed class NetworkCommands
{
    [Command("ping")]
    public async Task PingCommand([Argument(Description =
        "The target, or a file containing a list of targets, to ping.")] string target,
        [Option('c', Description = "How many pings to run.")] int count = 5,
        [Option('p', Description = "The protocol to use e.g ICMP (Default), TCP (Use only if the target is not replaying to ping messages.)")] string protocol = "ICMP",
        [Option('t', Description = "How long to wait for the target to replay.")] int timeout = 5000,
        [Option('s', Description = "The size of the packet to send.")] int bufferSize = 0,
        [Option('f', Description = "Flood the target with packets.")] bool flood = false,
        [Option('v', Description = "Enables verbose output.")] bool verboseOutput = false)
    {
        if (flood) {
            SmartConsole.LogInfo($"Flood mode is enabled. Disable ping outputs");

            if (count > 0 && count < 1000) {
                SmartConsole.LogInfo($"Count is less than 1000. Flood modes works best if count is -1.");
            }

            SmartConsole.Lock();
        }

        if (File.Exists(target)) {
            var targets = await File.ReadAllLinesAsync(target);

            if (count == -1) {
                while (true) {
                    foreach (var host in targets) {
                        SmartConsole.LogInfo($"Pinging {host}.\n");

                        await PingService.SimplePing(host, protocol, timeout, verboseOutput: verboseOutput);
                    }

                    if (!flood) await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            else {
                foreach (var host in targets) {
                    SmartConsole.LogInfo($"Pinging {host}.\n");

                    for (int i = 0; i < count; i++) {
                        await PingService.SimplePing(host, protocol, timeout, verboseOutput: verboseOutput);

                        if (!flood) await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            }
        }
        else {
            if (count == -1) {
                while (true) {
                    await PingService.SimplePing(target, protocol, timeout, verboseOutput: verboseOutput);

                    if (!flood) await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            for (int i = 0; i < count; i++) {
                await PingService.SimplePing(target, protocol, timeout, verboseOutput: verboseOutput);

                if (!flood) await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}