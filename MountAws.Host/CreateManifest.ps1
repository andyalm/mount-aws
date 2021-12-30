param(
    [Parameter(Position=0, Mandatory=$true)]
    $Directory
)

$ErrorActionPreference='Stop'

$ModuleVersion='0.0.1'
if($env:GITHUB_REF_NAME -and $env:GITHUB_REF_NAME -match '^v(?<Version>\d+\.\d+\.\d+)$') {
    $ModuleVersion = $Matches.Version
}

$FormatFiles = Get-ChildItem $Directory -Recurse |
    Where-Object { $_.Name -eq 'Formats.ps1xml' } |
    ForEach-Object {
        $ServiceName = $_ | Split-Path -Parent | Split-Path -Leaf
        
        "Services/$ServiceName/$($_.Name)"
    }

New-ModuleManifest -Path $(Join-Path $Directory MountAws.psd1) `
    -RootModule 'MountAws.Host.dll' `
    -ModuleVersion $ModuleVersion `
    -Guid 'ae6c3726-860c-4506-8acc-eed7930ead7f' `
    -Author 'Andy Alm' `
    -Copyright '(c) 2021 Andy Alm. All rights reserved.' `
    -Description 'An experimental powershell provider that allows you to browse various aws services as a filesystem' `
    -PowerShellVersion '7.2' `
    -FormatsToProcess $FormatFiles `
    -RequiredModules @() `
    -FunctionsToExport @() `
    -VariablesToExport @() `
    -CmdletsToExport @() `
    -AliasesToExport @()