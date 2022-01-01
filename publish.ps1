$ErrorActionPreference='Stop'

dotnet publish MountAws.Host
Get-ChildItem -Recurse ./bin/MountAws | Select-Object Name
Publish-Module -Path ./bin/MountAws -NuGetApiKey $env:NuGetApiKey