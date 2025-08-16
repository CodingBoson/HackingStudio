# Build and install HackingStudio on Windows.

dotnet pack -c Release HackingStudio.csproj
dotnet tool uninstall HackingStudio --global > $NULL
dotnet tool install --global --add-source .\bin\nupkg HackingStudio --version 1.0.0
