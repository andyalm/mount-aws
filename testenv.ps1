param(
    [switch]
    $Debug
)
$env:NO_MOUNT_AWS='1'
$DebugPreference=$Debug ? 'Continue' : 'SilentlyContinue'
dotnet build
pwsh -Interactive -NoProfile -NoExit -c "Set-Variable -Scope Global -Name DebugPreference -Value $DebugPreference;New-Alias ls Get-ChildItem;New-Alias cat Get-Content;Import-Module $PWD/MountAws.Host/bin/Debug/net6.0/MountAws.psd1 && cd aws:"
