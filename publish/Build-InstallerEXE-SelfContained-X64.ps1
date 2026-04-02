param(
    [switch]$SkipPublish
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'Pack.Common.ps1')

New-NmsInstallerExe `
    -ProfileKind 'SelfContained' `
    -VariantId 'SelfContained-X64' `
    -AppId '{6E3EEA6B-585F-41E6-A95F-A6E44B80A70B}' `
    -AppName 'NMSShipIOTool SelfContained x64' `
    -DefaultInstallSubdir 'NMSShipIOTool SelfContained x64' `
    -SkipPublish:$SkipPublish
