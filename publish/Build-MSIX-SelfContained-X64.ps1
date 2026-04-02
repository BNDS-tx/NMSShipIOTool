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
    -ProfileKind 'SelfContained' `
    -VariantId 'SelfContained-X64' `
    -PackageIdentityName 'NMSShipIOTool.SelfContained.X64' `
    -DisplayName 'NMSShipIOTool SelfContained x64' `
    -Publisher $Publisher `
    -PublisherDisplayName $PublisherDisplayName `
    -CertificateThumbprint $CertificateThumbprint `
    -PfxPath $PfxPath `
    -PfxPassword $PfxPassword `
    -SkipPublish:$SkipPublish
