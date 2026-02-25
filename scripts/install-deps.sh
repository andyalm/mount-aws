#!/bin/bash
set -e

# Skip in local environments
if [ -z "$CLAUDE_CODE_REMOTE" ] || [ "$CLAUDE_CODE_REMOTE" != "true" ]; then
  exit 0
fi

# Skip if already installed
if command -v dotnet &>/dev/null && command -v pwsh &>/dev/null; then
  exit 0
fi

apt-get update

# Install .NET 8 SDK
if ! command -v dotnet &>/dev/null; then
  apt-get install -y dotnet-sdk-8.0
fi

# Install PowerShell (requires Microsoft package repo)
if ! command -v pwsh &>/dev/null; then
  curl -sSL https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -o /tmp/packages-microsoft-prod.deb
  dpkg -i /tmp/packages-microsoft-prod.deb
  apt-get update
  apt-get install -y powershell
  rm -f /tmp/packages-microsoft-prod.deb
fi
