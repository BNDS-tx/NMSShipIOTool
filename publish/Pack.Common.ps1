Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-NmsRepoRoot {
    return (Split-Path -Parent $PSScriptRoot)
}

function Get-NmsProjectDir {
    return (Join-Path (Get-NmsRepoRoot) 'NMSShipIOTool')
}

function Get-NmsProjectPath {
    return (Join-Path (Get-NmsProjectDir) 'NMSShipIOTool.csproj')
}

function Get-NmsLogoIcoPath {
    return (Join-Path (Get-NmsProjectDir) 'Resources\logo.ico')
}

function Get-NmsPublishRoot {
    return (Join-Path (Get-NmsRepoRoot) 'publish')
}

function Find-NmsPublishProfilePath {
    param(
        [Parameter(Mandatory)]
        [ValidateSet('SelfContained', 'FrameworkDependent')]
        [string]$ProfileKind
    )

    $profilesDir = Join-Path (Get-NmsProjectDir) 'Properties\PublishProfiles'
    $profileFiles = Get-ChildItem -Path $profilesDir -Filter '*.pubxml' -File
    if (-not $profileFiles) {
        throw "No .pubxml files found under '$profilesDir'."
    }

    $desiredSelfContained = ($ProfileKind -eq 'SelfContained')
    $matches = @(foreach ($file in $profileFiles) {
        [xml]$xml = Get-Content -Path $file.FullName -Encoding UTF8
        $selfContainedText = @($xml.Project.PropertyGroup | ForEach-Object { $_.SelfContained } | Where-Object { $_ })[0]
        if (-not $selfContainedText) {
            continue
        }

        $isSelfContained = [System.Convert]::ToBoolean($selfContainedText)
        if ($isSelfContained -eq $desiredSelfContained) {
            $file.FullName
        }
    })

    if (-not $matches) {
        throw "Could not find a publish profile for '$ProfileKind'."
    }

    if ($matches.Count -gt 1) {
        throw "Found multiple publish profiles for '$ProfileKind': $($matches -join ', ')"
    }

    return $matches[0]
}

function Get-NmsPublishDirFromProfile {
    param(
        [Parameter(Mandatory)]
        [ValidateSet('SelfContained', 'FrameworkDependent')]
        [string]$ProfileKind
    )

    $profilePath = Find-NmsPublishProfilePath -ProfileKind $ProfileKind
    if (-not (Test-Path -LiteralPath $profilePath)) {
        throw "Publish profile not found: $profilePath"
    }

    [xml]$xml = Get-Content -Path $profilePath -Encoding UTF8
    $publishDir = @($xml.Project.PropertyGroup | ForEach-Object { $_.PublishDir } | Where-Object { $_ })[0]
    if (-not $publishDir) {
        throw "Could not read <PublishDir> from '$profilePath'."
    }

    if ([System.IO.Path]::IsPathRooted($publishDir)) {
        return $publishDir
    }

    return [System.IO.Path]::GetFullPath((Join-Path (Get-NmsProjectDir) $publishDir))
}

function Get-NmsVersionInfo {
    $projectPath = Get-NmsProjectPath
    [xml]$xml = Get-Content -Path $projectPath -Encoding UTF8
    $rawVersion = @($xml.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ })[0]
    if (-not $rawVersion) {
        throw "Could not read <Version> from '$projectPath'."
    }

    $msixVersion = if ($rawVersion -match '^\d+\.\d+\.\d+$') {
        "$rawVersion.0"
    }
    elseif ($rawVersion -match '^\d+\.\d+\.\d+\.\d+$') {
        $rawVersion
    }
    else {
        throw "Project version '$rawVersion' is not compatible with MSIX version format."
    }

    return [pscustomobject]@{
        DisplayVersion = $rawVersion
        MsixVersion    = $msixVersion
    }
}

function Ensure-NmsDirectory {
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }

    return $Path
}

function Reset-NmsDirectory {
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }

    New-Item -ItemType Directory -Path $Path -Force | Out-Null
    return $Path
}

function Find-NmsVsMsBuild {
    $vsWhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vsWhere) {
        $msbuild = & $vsWhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
        if ($msbuild -and (Test-Path -LiteralPath $msbuild)) {
            return $msbuild
        }
    }

    $fallbacks = @(
        'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
    )

    foreach ($path in $fallbacks) {
        if (Test-Path -LiteralPath $path) {
            return $path
        }
    }

    throw 'Could not locate Visual Studio MSBuild.exe.'
}

function Find-NmsInnoSetupCompiler {
    $candidates = @()

    if ($env:INNO_SETUP_COMPILER) {
        $candidates += $env:INNO_SETUP_COMPILER
    }

    $candidates += @(
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe')
    )

    foreach ($path in $candidates) {
        if ($path -and (Test-Path -LiteralPath $path)) {
            return $path
        }
    }

    throw 'Could not locate Inno Setup compiler. Install Inno Setup 6 or set INNO_SETUP_COMPILER.'
}

function Find-NmsWindowsSdkTool {
    param(
        [Parameter(Mandatory)]
        [string]$ToolName
    )

    $kitsRoot = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
    if (-not (Test-Path -LiteralPath $kitsRoot)) {
        throw "Windows SDK bin directory not found: $kitsRoot"
    }

    $matches = Get-ChildItem -Path $kitsRoot -Recurse -Filter $ToolName -File |
        Where-Object { $_.FullName -match '\\x64\\' } |
        Sort-Object FullName -Descending

    if (-not $matches) {
        throw "Could not locate $ToolName under '$kitsRoot'."
    }

    return $matches[0].FullName
}

function Invoke-NmsVsPublish {
    param(
        [Parameter(Mandatory)]
        [ValidateSet('SelfContained', 'FrameworkDependent')]
        [string]$ProfileKind
    )

    $projectPath = Get-NmsProjectPath
    $profilePath = Find-NmsPublishProfilePath -ProfileKind $ProfileKind
    if (-not (Test-Path -LiteralPath $profilePath)) {
        throw "Publish profile not found: $profilePath"
    }

    Assert-NmsAppNotRunning

    $msbuild = Find-NmsVsMsBuild
    Write-Host "Publishing with MSBuild profile '$profilePath'..."
    & $msbuild $projectPath /restore /t:Publish "/p:PublishProfile=$profilePath"
    if ($LASTEXITCODE -ne 0) {
        throw "MSBuild publish failed for profile '$profilePath'."
    }
}

function Assert-NmsPublishSourceExists {
    param(
        [Parameter(Mandatory)]
        [string]$SourceDir
    )

    if (-not (Test-Path -LiteralPath $SourceDir)) {
        throw "Publish output folder not found: $SourceDir"
    }

    $files = Get-ChildItem -LiteralPath $SourceDir -Recurse -File -ErrorAction Stop
    if (-not $files) {
        throw "Publish output folder is empty: $SourceDir"
    }
}

function Assert-NmsAppNotRunning {
    $running = Get-Process -Name 'NMSShipIOTool' -ErrorAction SilentlyContinue
    if ($running) {
        $ids = ($running | Select-Object -ExpandProperty Id) -join ', '
        throw "NMSShipIOTool.exe is still running (PID: $ids). Close it before publishing or rerun the script with -SkipPublish."
    }
}

function ConvertTo-NmsInnoValue {
    param(
        [Parameter(Mandatory)]
        [string]$Value
    )

    return $Value.Replace('{', '{{').Replace('"', '""')
}

function New-NmsInstallerExe {
    param(
        [Parameter(Mandatory)]
        [ValidateSet('SelfContained', 'FrameworkDependent')]
        [string]$ProfileKind,

        [Parameter(Mandatory)]
        [string]$VariantId,

        [Parameter(Mandatory)]
        [string]$AppId,

        [Parameter(Mandatory)]
        [string]$AppName,

        [Parameter(Mandatory)]
        [string]$DefaultInstallSubdir,

        [switch]$SkipPublish
    )

    if (-not $SkipPublish) {
        Invoke-NmsVsPublish -ProfileKind $ProfileKind
    }

    $version = Get-NmsVersionInfo
    $sourceDir = Get-NmsPublishDirFromProfile -ProfileKind $ProfileKind
    Assert-NmsPublishSourceExists -SourceDir $sourceDir

    $packagesDir = Ensure-NmsDirectory (Join-Path (Get-NmsPublishRoot) 'packages\exe')
    $tempDir = Reset-NmsDirectory (Join-Path (Get-NmsPublishRoot) ("_tmp\{0}-exe" -f $VariantId))
    $iconPath = Get-NmsLogoIcoPath
    $outputBaseName = "NMSShipIOTool-{0}-Setup-{1}" -f $VariantId, $version.DisplayVersion
    $issPath = Join-Path $tempDir "$outputBaseName.iss"

    $template = @'
[Setup]
AppId=__APP_ID__
AppName=__APP_NAME__
AppVersion=__APP_VERSION__
AppPublisher=Bluen
DefaultDirName={autopf}\__DEFAULT_SUBDIR__
DisableProgramGroupPage=yes
OutputDir=__OUTPUT_DIR__
OutputBaseFilename=__OUTPUT_BASE__
SetupIconFile=__ICON_PATH__
UninstallDisplayIcon={app}\NMSShipIOTool.exe
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "__SOURCE_DIR__\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\__APP_NAME__"; Filename: "{app}\NMSShipIOTool.exe"
Name: "{autodesktop}\__APP_NAME__"; Filename: "{app}\NMSShipIOTool.exe"

[Run]
Filename: "{app}\NMSShipIOTool.exe"; Description: "Launch __APP_NAME__"; Flags: nowait postinstall skipifsilent
'@

    $issContent = $template.
        Replace('__APP_ID__', (ConvertTo-NmsInnoValue -Value $AppId)).
        Replace('__APP_NAME__', (ConvertTo-NmsInnoValue -Value $AppName)).
        Replace('__APP_VERSION__', (ConvertTo-NmsInnoValue -Value $version.DisplayVersion)).
        Replace('__DEFAULT_SUBDIR__', (ConvertTo-NmsInnoValue -Value $DefaultInstallSubdir)).
        Replace('__OUTPUT_DIR__', (ConvertTo-NmsInnoValue -Value $packagesDir)).
        Replace('__OUTPUT_BASE__', (ConvertTo-NmsInnoValue -Value $outputBaseName)).
        Replace('__ICON_PATH__', (ConvertTo-NmsInnoValue -Value $iconPath)).
        Replace('__SOURCE_DIR__', (ConvertTo-NmsInnoValue -Value $sourceDir))

    [System.IO.File]::WriteAllText($issPath, $issContent, [System.Text.UTF8Encoding]::new($false))

    $iscc = Find-NmsInnoSetupCompiler
    & $iscc $issPath
    if ($LASTEXITCODE -ne 0) {
        throw "Inno Setup build failed for '$issPath'."
    }

    $setupPath = Join-Path $packagesDir ($outputBaseName + '.exe')
    Write-Host "Installer EXE created: $setupPath"
    return $setupPath
}

function New-NmsMsixVisualAssets {
    param(
        [Parameter(Mandatory)]
        [string]$IconPath,

        [Parameter(Mandatory)]
        [string]$AssetsDir
    )

    Ensure-NmsDirectory -Path $AssetsDir | Out-Null
    Add-Type -AssemblyName System.Drawing

    $sizes = @(
        @{ Name = 'Square44x44Logo.png'; Size = 44 },
        @{ Name = 'Square150x150Logo.png'; Size = 150 },
        @{ Name = 'StoreLogo.png'; Size = 50 }
    )

    foreach ($item in $sizes) {
        $bitmap = New-Object System.Drawing.Bitmap $item.Size, $item.Size
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $graphics.Clear([System.Drawing.Color]::Transparent)

        try {
            $icon = New-Object System.Drawing.Icon $IconPath, $item.Size, $item.Size
            $graphics.DrawIcon($icon, (New-Object System.Drawing.Rectangle 0, 0, $item.Size, $item.Size))
        }
        finally {
            if ($icon) { $icon.Dispose() }
            $graphics.Dispose()
        }

        $targetPath = Join-Path $AssetsDir $item.Name
        $bitmap.Save($targetPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $bitmap.Dispose()
    }
}

function Get-OrCreate-NmsCodeSigningCertificate {
    param(
        [Parameter(Mandatory)]
        [string]$Publisher,

        [Parameter(Mandatory)]
        [string]$FriendlyName,

        [Parameter(Mandatory)]
        [string]$ExportDir
    )

    $cert = Get-ChildItem Cert:\CurrentUser\My |
        Where-Object { $_.Subject -eq $Publisher -and $_.HasPrivateKey } |
        Sort-Object NotAfter -Descending |
        Select-Object -First 1

    if (-not $cert) {
        Write-Host "Creating self-signed code signing certificate for $Publisher ..."
        $cert = New-SelfSignedCertificate `
            -Type CodeSigningCert `
            -Subject $Publisher `
            -FriendlyName $FriendlyName `
            -CertStoreLocation 'Cert:\CurrentUser\My' `
            -NotAfter (Get-Date).AddYears(3)
    }

    Ensure-NmsDirectory -Path $ExportDir | Out-Null
    $cerPath = Join-Path $ExportDir ($FriendlyName + '.cer')
    Export-Certificate -Cert $cert -FilePath $cerPath -Force | Out-Null

    return [pscustomobject]@{
        Certificate = $cert
        CerPath     = $cerPath
    }
}

function Sign-NmsMsixPackage {
    param(
        [Parameter(Mandatory)]
        [string]$PackagePath,

        [string]$CertificateThumbprint,
        [string]$PfxPath,
        [string]$PfxPassword,
        [string]$Publisher,
        [string]$FriendlyName,
        [string]$CertificateExportDir
    )

    $signtool = Find-NmsWindowsSdkTool -ToolName 'signtool.exe'

    if ($PfxPath) {
        if (-not (Test-Path -LiteralPath $PfxPath)) {
            throw "PFX file not found: $PfxPath"
        }

        $args = @('sign', '/fd', 'SHA256', '/f', $PfxPath)
        if ($PfxPassword) {
            $args += @('/p', $PfxPassword)
        }
        $args += $PackagePath

        & $signtool @args
        if ($LASTEXITCODE -ne 0) {
            throw "SignTool failed while signing '$PackagePath' with PFX."
        }

        return $null
    }

    if ($CertificateThumbprint) {
        & $signtool sign /fd SHA256 /sha1 $CertificateThumbprint /s My $PackagePath
        if ($LASTEXITCODE -ne 0) {
            throw "SignTool failed while signing '$PackagePath' with certificate thumbprint '$CertificateThumbprint'."
        }

        return $null
    }

    $certInfo = Get-OrCreate-NmsCodeSigningCertificate `
        -Publisher $Publisher `
        -FriendlyName $FriendlyName `
        -ExportDir $CertificateExportDir

    & $signtool sign /fd SHA256 /sha1 $certInfo.Certificate.Thumbprint /s My $PackagePath
    if ($LASTEXITCODE -ne 0) {
        throw "SignTool failed while signing '$PackagePath' with the generated self-signed certificate."
    }

    return $certInfo.CerPath
}

function New-NmsMsixPackage {
    param(
        [Parameter(Mandatory)]
        [ValidateSet('SelfContained', 'FrameworkDependent')]
        [string]$ProfileKind,

        [Parameter(Mandatory)]
        [string]$VariantId,

        [Parameter(Mandatory)]
        [string]$PackageIdentityName,

        [Parameter(Mandatory)]
        [string]$DisplayName,

        [string]$Publisher = 'CN=NMSShipIOTool Test',
        [string]$PublisherDisplayName = 'NMSShipIOTool Test',
        [string]$CertificateThumbprint,
        [string]$PfxPath,
        [string]$PfxPassword,
        [switch]$SkipPublish
    )

    if (-not $SkipPublish) {
        Invoke-NmsVsPublish -ProfileKind $ProfileKind
    }

    $version = Get-NmsVersionInfo
    $sourceDir = Get-NmsPublishDirFromProfile -ProfileKind $ProfileKind
    Assert-NmsPublishSourceExists -SourceDir $sourceDir

    $packagesDir = Ensure-NmsDirectory (Join-Path (Get-NmsPublishRoot) 'packages\msix')
    $certsDir = Ensure-NmsDirectory (Join-Path $packagesDir 'certificates')
    $stageDir = Reset-NmsDirectory (Join-Path (Get-NmsPublishRoot) ("_tmp\{0}-msix" -f $VariantId))

    Copy-Item -Path (Join-Path $sourceDir '*') -Destination $stageDir -Recurse -Force

    $assetsDir = Join-Path $stageDir 'Assets'
    New-NmsMsixVisualAssets -IconPath (Get-NmsLogoIcoPath) -AssetsDir $assetsDir

    $manifestPath = Join-Path $stageDir 'AppxManifest.xml'
    $manifestTemplate = @'
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">
  <Identity
    Name="__IDENTITY_NAME__"
    Publisher="__PUBLISHER__"
    Version="__VERSION__"
    ProcessorArchitecture="x64" />
  <Properties>
    <DisplayName>__DISPLAY_NAME__</DisplayName>
    <PublisherDisplayName>__PUBLISHER_DISPLAY__</PublisherDisplayName>
    <Logo>Assets\StoreLogo.png</Logo>
  </Properties>
  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.26100.0" />
  </Dependencies>
  <Resources>
    <Resource Language="x-generate" />
  </Resources>
  <Applications>
    <Application Id="App" Executable="NMSShipIOTool.exe" EntryPoint="Windows.FullTrustApplication">
      <uap:VisualElements
        DisplayName="__DISPLAY_NAME__"
        Description="__DISPLAY_NAME__"
        BackgroundColor="transparent"
        Square150x150Logo="Assets\Square150x150Logo.png"
        Square44x44Logo="Assets\Square44x44Logo.png" />
    </Application>
  </Applications>
  <Capabilities>
    <rescap:Capability Name="runFullTrust" />
  </Capabilities>
</Package>
'@

    $manifestContent = $manifestTemplate.
        Replace('__IDENTITY_NAME__', $PackageIdentityName).
        Replace('__PUBLISHER__', $Publisher).
        Replace('__VERSION__', $version.MsixVersion).
        Replace('__DISPLAY_NAME__', $DisplayName).
        Replace('__PUBLISHER_DISPLAY__', $PublisherDisplayName)

    [System.IO.File]::WriteAllText($manifestPath, $manifestContent, [System.Text.UTF8Encoding]::new($false))

    $makeAppx = Find-NmsWindowsSdkTool -ToolName 'makeappx.exe'
    $packagePath = Join-Path $packagesDir ("NMSShipIOTool-{0}-{1}.msix" -f $VariantId, $version.DisplayVersion)
    if (Test-Path -LiteralPath $packagePath) {
        Remove-Item -LiteralPath $packagePath -Force
    }

    & $makeAppx pack /d $stageDir /p $packagePath /o
    if ($LASTEXITCODE -ne 0) {
        throw "makeappx failed while packaging '$stageDir'."
    }

    $certPath = Sign-NmsMsixPackage `
        -PackagePath $packagePath `
        -CertificateThumbprint $CertificateThumbprint `
        -PfxPath $PfxPath `
        -PfxPassword $PfxPassword `
        -Publisher $Publisher `
        -FriendlyName $PackageIdentityName `
        -CertificateExportDir $certsDir

    Write-Host "MSIX created: $packagePath"
    if ($certPath) {
        Write-Host "Certificate exported: $certPath"
        Write-Host 'Target machines must trust this .cer before installing the MSIX.'
    }

    return $packagePath
}
