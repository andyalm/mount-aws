$ErrorActionPreference='Stop'

dotnet publish MountAws.Host
Publish-Module -Path ./bin/MountAws -NuGetApiKey $env:NuGetApiKey