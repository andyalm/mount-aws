$ErrorActionPreference='Stop'

dotnet publish MountAws
Publish-Module -Path ./bin/MountAws -NuGetApiKey $env:NuGetApiKey