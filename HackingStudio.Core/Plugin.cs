using Realtin.Xdsl.Serialization;

namespace HackingStudio.Core;

public sealed class Plugin
{
    [XdslRequired] public required string Name { get; set; }

    [XdslRequired] public required string Description { get; set; }

    [XdslRequired] public required string Command { get; set; }

    [XdslRequired] public List<string> UnixFileModes { get; set; } = [];
}
