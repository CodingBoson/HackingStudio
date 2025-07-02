using Cocona;
using HackingStudio.CLI.Forensics;

namespace HackingStudio.CLI;

[HasSubCommands(typeof(ForensicsCommands), "forensics", Description = "Commands for reverse engineering. (Alias: fcs)")]
public sealed class ForensicsCommandsSection;