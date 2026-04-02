param(
    [ValidateSet('All', 'SelfContained', 'FrameworkDependent')]
    [string]$ProfileSet = 'All',

    [ValidateSet('All', 'Exe', 'Msix')]
    [string]$PackageSet = 'All',

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

function Test-BuildKindSelected {
    param(
        [Parameter(Mandatory)]
        [string]$Wanted
    )

    return $ProfileSet -eq 'All' -or $ProfileSet -eq $Wanted
}

function Test-PackageKindSelected {
    param(
        [Parameter(Mandatory)]
        [string]$Wanted
    )

    return $PackageSet -eq 'All' -or $PackageSet -eq $Wanted
}

$results = [System.Collections.Generic.List[string]]::new()

function Build-ProfilePackages {
    param(
        [Parameter(Mandatory)]
        [ValidateSet('SelfContained', 'FrameworkDependent')]
        [string]$ProfileKind
    )

    $profileDisplay = if ($ProfileKind -eq 'SelfContained') { 'SelfContained x64' } else { 'FrameworkDependent x64' }

    if (-not $SkipPublish) {
        Write-Host "Publishing $profileDisplay ..."
        Invoke-NmsVsPublish -ProfileKind $ProfileKind
    }

    if (Test-PackageKindSelected -Wanted 'Exe') {
        $exePath = if ($ProfileKind -eq 'SelfContained') {
            New-NmsInstallerExe `
                -ProfileKind 'SelfContained' `
                -VariantId 'SelfContained-X64' `
                -AppId '{6E3EEA6B-585F-41E6-A95F-A6E44B80A70B}' `
                -AppName 'NMSShipIOTool SelfContained x64' `
                -DefaultInstallSubdir 'NMSShipIOTool SelfContained x64' `
                -SkipPublish
        }
        else {
            New-NmsInstallerExe `
                -ProfileKind 'FrameworkDependent' `
                -VariantId 'FrameworkDependent-X64' `
                -AppId '{6E22E0B9-10DF-4CB8-A5E8-79EDE3F3389A}' `
                -AppName 'NMSShipIOTool FrameworkDependent x64' `
                -DefaultInstallSubdir 'NMSShipIOTool FrameworkDependent x64' `
                -SkipPublish
        }

        $results.Add("EXE [$profileDisplay]: $exePath")
    }

    if (Test-PackageKindSelected -Wanted 'Msix') {
        $msixPath = if ($ProfileKind -eq 'SelfContained') {
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
                -SkipPublish
        }
        else {
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
                -SkipPublish
        }

        $results.Add("MSIX [$profileDisplay]: $msixPath")
    }
}

if (Test-BuildKindSelected -Wanted 'SelfContained') {
    Build-ProfilePackages -ProfileKind 'SelfContained'
}

if (Test-BuildKindSelected -Wanted 'FrameworkDependent') {
    Build-ProfilePackages -ProfileKind 'FrameworkDependent'
}

Write-Host ''
Write-Host 'Build complete:'
$results | ForEach-Object { Write-Host "  $_" }
