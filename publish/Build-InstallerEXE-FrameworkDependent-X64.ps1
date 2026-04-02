param(
    [switch]$SkipPublish
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'Pack.Common.ps1')

New-NmsInstallerExe `
    -ProfileKind 'FrameworkDependent' `
    -VariantId 'FrameworkDependent-X64' `
    -AppId '{6E22E0B9-10DF-4CB8-A5E8-79EDE3F3389A}' `
    -AppName 'NMSShipIOTool FrameworkDependent x64' `
    -DefaultInstallSubdir 'NMSShipIOTool FrameworkDependent x64' `
    -SkipPublish:$SkipPublish
