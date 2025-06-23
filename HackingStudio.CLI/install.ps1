# Build and install HackingStudio.CLI on Windows.

dotnet pack -c Release HackingStudio.CLI.csproj
dotnet tool uninstall HackingStudio.CLI --global > $NULL
dotnet tool install --global --add-source .\bin\nupkg HackingStudio.CLI --version 1.0.0
