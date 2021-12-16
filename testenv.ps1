param(
    [switch]
    $Debug
)

$DebugPreference=$Debug ? 'Continue' : 'SilentlyContinue'
dotnet build
pwsh -Interactive -NoProfile -NoExit -c "Set-Variable -Scope Global -Name DebugPreference -Value $DebugPreference;New-Alias ls Get-ChildItem;New-Alias cat Get-Content;Import-Module $PWD/MountAws/bin/Debug/net6.0/MountAws.psd1 && cd aws:"
