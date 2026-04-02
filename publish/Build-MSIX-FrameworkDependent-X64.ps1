param(
    [string]$Publisher = 'CN=NMSShipIOTool Test',
    [string]$PublisherDisplayName = 'NMSShipIOTool Test',
    [string]$CertificateThumbprint,
    [string]$PfxPath,
    [string]$PfxPassword,
    [switch]$SkipPublish
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'Pack.Common.ps1')

New-NmsMsixPackage `
    -ProfileKind 'FrameworkDependent' `
    -VariantId 'FrameworkDependent-X64' `
    -PackageIdentityName 'NMSShipIOTool.FrameworkDependent.X64' `
    -DisplayName 'NMSShipIOTool FrameworkDependent x64' `
    -Publisher $Publisher `
    -PublisherDisplayName $PublisherDisplayName `
    -CertificateThumbprint $CertificateThumbprint `
    -PfxPath $PfxPath `
    -PfxPassword $PfxPassword `
    -SkipPublish:$SkipPublish
