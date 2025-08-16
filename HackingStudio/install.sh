#!/bin/bash

# Build and install HackingStudio on Linux/MacOS.

dotnet pack -c Release HackingStudio.csproj
dotnet tool uninstall HackingStudio --global > /dev/null
dotnet tool install --global --add-source ./bin/nupkg HackingStudio --version 3.0.0

# TODO: Create a new alias 'hs'.

if ! grep -q "alias hs=" ~/.bashrc; then
    echo "alias hs='hacking-studio'" >> ~/.bashrc
    
    echo "HackingStudio installed successfully. Please restart your terminal or run 'source ~/.bashrc' to apply the changes."
fi
