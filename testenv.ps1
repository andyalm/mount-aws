#!/usr/bin/env pwsh -NoExit -Interactive -NoLogo -NoProfile

param(
    [switch]
    $Debug
)
$ErrorActionPreference='Stop'
$env:NO_MOUNT_AWS='1'
$DebugPreference=$Debug ? 'Continue' : 'SilentlyContinue'
dotnet build
if(-not (Get-Alias ls -ErrorAction SilentlyContinue)) {
    New-Alias ls Get-ChildItem
}
if(-not (Get-Alias cat -ErrorAction SilentlyContinue)) {
    New-Alias cat Get-Content
}
Import-Module $PWD/MountAws/bin/Debug/net6.0/Host/MountAws.psd1
cd aws: