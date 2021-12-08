param(
    [switch]
    $Debug
)

$DebugPreference=$Debug ? 'Continue' : 'SilentlyContinue'
dotnet build
pwsh -Interactive -NoExit -c "Set-Variable -Scope Global -Name DebugPreference -Value $DebugPreference;Import-Module $PWD/MountAws/bin/Debug/net6.0/MountAws.psd1 && cd aws:"
