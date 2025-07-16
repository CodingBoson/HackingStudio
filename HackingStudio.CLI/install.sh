#!/bin/bash

# Build and install HackingStudio.CLI on Linux/MacOS.

dotnet pack -c Release HackingStudio.CLI.csproj
dotnet tool uninstall HackingStudio.CLI --global > /dev/null
dotnet tool install --global --add-source ./bin/nupkg HackingStudio.CLI --version 1.0.0

# TODO: Create a new alias 'hs'.

if ! grep -q "alias hs=" ~/.bashrc; then
    echo "alias hs='hacking-studio'" >> ~/.bashrc
fi

echo "HackingStudio.CLI installed successfully. Please restart your terminal or run 'source ~/.bashrc' to apply the changes."
