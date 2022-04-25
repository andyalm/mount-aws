$ErrorActionPreference='Stop'

dotnet publish MountAws
Get-ChildItem -Recurse ./bin/MountAws | Select-Object Name
Publish-Module -Path ./bin/MountAws -NuGetApiKey $env:NuGetApiKey