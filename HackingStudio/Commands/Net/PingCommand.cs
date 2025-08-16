using BosonWare.TerminalApp;
using CommandLine;
using HackingStudio.Core.Networking;

namespace HackingStudio.Commands.Net;

[Group("net", Description = "Network command.")]
[Command("ping", Description = "Ping a host/hosts using ICMP or TCP protocols.")]
public sealed class PingCommand : Command<PingCommand.Options>
{
    public override async Task Execute(Options options)
    {
        if (options.Flood) {
            SmartConsole.LogInfo("Flood mode is enabled. Disabling ping outputs");

            if (options.Count > 0 && options.Count < 1000) {
                SmartConsole.LogInfo("Count is less than 1000. Flood modes works best if count is -1.");
            }

            SmartConsole.Lock();
        }

        if (File.Exists(options.Target)) {
            var targets = await File.ReadAllLinesAsync(options.Target);

            if (options.Count == -1) {
                while (true) {
                    foreach (var host in targets) {
                        SmartConsole.LogInfo($"Pinging {host}.\n");

                        await PingService.SimplePing(
                            host,
                            options.Protocol,
                            options.Timeout,
                            verboseOutput: options.VerboseOutput);
                    }

                    if (!options.Flood) await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            foreach (var host in targets) {
                SmartConsole.LogInfo($"Pinging {host}.\n");

                for (var i = 0; i < options.Count; i++) {
                    await PingService.SimplePing(
                        host,
                        options.Protocol,
                        options.Timeout,
                        verboseOutput: options.VerboseOutput);

                    if (!options.Flood) await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
        else {
            if (options.Count == -1) {
                while (true) {
                    await PingService.SimplePing(
                        options.Target,
                        options.Protocol,
                        options.Timeout,
                        verboseOutput: options.VerboseOutput);

                    if (!options.Flood) await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            for (var i = 0; i < options.Count; i++) {
                await PingService.SimplePing(
                    options.Target,
                    options.Protocol,
                    options.Timeout,
                    verboseOutput: options.VerboseOutput);

                if (!options.Flood) await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

    public sealed class Options
    {
        [Value(0, HelpText =
            "The target, or a file containing a list of targets, to ping.")]
        public string Target { get; set; }

        [Option('c', HelpText = "How many pings to run.")]
        public int Count { get; set; } = 5;

        [Option('p', HelpText = "The protocol to use e.g ICMP (Default), TCP (Use only if the target is not replaying to ping messages.)")]
        public PingProtocol Protocol { get; set; } = PingProtocol.ICMP;

        [Option('t', HelpText = "How long to wait for the target to replay.")]
        public int Timeout { get; set; } = 5000;

        [Option('s', HelpText = "The size of the packet to send.")]
        public int BufferSize { get; set; } = 0;

        [Option('f', HelpText = "Flood the target with packets.")]
        public bool Flood { get; set; } = false;

        [Option('v', HelpText = "Enables verbose output.")]
        public bool VerboseOutput { get; set; } = false;
    }
}
