$ErrorActionPreference = "Stop";
$ProgressPreference = "SilentlyContinue"

# Set the root of the repository
$repoRoot = Resolve-Path "$PSScriptRoot/."

Write-Host "Preparing your Sitecore Containers environment!" -ForegroundColor Green

# Parse .env values
$envFileLocation = Join-Path $repoRoot "./apps/xmcloud/.env"
$envContent = Get-Content $envFileLocation -Encoding UTF8
$xmCloudHost = $envContent | Where-Object { $_ -imatch "^CM_HOST=.+" } | ForEach-Object { $_.Substring($_.IndexOf("=") + 1) }

################################################
# Retrieve and import SitecoreDockerTools module
################################################

# Check for Sitecore Gallery
Import-Module PowerShellGet
$SitecoreGallery = Get-PSRepository | Where-Object { $_.SourceLocation -eq "https://nuget.sitecore.com/resources/v2" }
if (-not $SitecoreGallery)
{
    Write-Host "Adding Sitecore PowerShell Gallery..." -ForegroundColor Green
    Unregister-PSRepository -Name SitecoreGallery -ErrorAction SilentlyContinue
    Register-PSRepository -Name SitecoreGallery -SourceLocation "https://nuget.sitecore.com/resources/v2" -InstallationPolicy Trusted
    $SitecoreGallery = Get-PSRepository -Name SitecoreGallery
}

# Install and Import SitecoreDockerTools
$dockerToolsVersion = "10.2.7"
Remove-Module SitecoreDockerTools -ErrorAction SilentlyContinue
if (-not (Get-InstalledModule -Name SitecoreDockerTools -RequiredVersion $dockerToolsVersion -ErrorAction SilentlyContinue))
{
    Write-Host "Installing SitecoreDockerTools..." -ForegroundColor Green
    Install-Module SitecoreDockerTools -RequiredVersion $dockerToolsVersion -Scope CurrentUser -Repository $SitecoreGallery.Name
}
Write-Host "Importing SitecoreDockerTools..." -ForegroundColor Green
Import-Module SitecoreDockerTools -RequiredVersion $dockerToolsVersion
Write-SitecoreDockerWelcome

##################################
# Configure TLS/HTTPS certificates
##################################

Push-Location (Join-Path $repoRoot "\docker\data\traefik\certs")
try
{
    $mkcert = ".\mkcert.exe"
    if ($null -ne (Get-Command mkcert.exe -ErrorAction SilentlyContinue))
    {
        # mkcert installed in PATH
        $mkcert = "mkcert"
    }
    elseif (-not (Test-Path $mkcert))
    {
        Write-Host "Downloading and installing mkcert certificate tool..." -ForegroundColor Green
        Invoke-WebRequest "https://github.com/FiloSottile/mkcert/releases/download/v1.4.4/mkcert-v1.4.4-windows-amd64.exe" -UseBasicParsing -OutFile mkcert.exe
        if ((Get-FileHash mkcert.exe).Hash -ne "D2660B50A9ED59EADA480750561C96ABC2ED4C9A38C6A24D93E30E0977631398")
        {
            Remove-Item mkcert.exe -Force
            throw "Invalid mkcert.exe file"
        }
    }
    Write-Host "Generating Traefik TLS certificate..." -ForegroundColor Green
    & $mkcert -install
    & $mkcert $xmCloudHost
    & $mkcert "*.2025-team-asmblii.localhost"

}
catch
{
    Write-Error "An error occurred while attempting to generate TLS certificate: $_"
}
finally
{
    Pop-Location
}

################################
# Add Windows hosts file entries
################################

Write-Host "Adding Windows hosts file entries..." -ForegroundColor Green

Add-HostsEntry $xmCloudHost
Add-HostsEntry "api-solr.2025-team-asmblii.localhost"
Add-HostsEntry "api-app.2025-team-asmblii.localhost"
Add-HostsEntry "headnextjss.2025-team-asmblii.localhost"
Add-HostsEntry "dashboard.2025-team-asmblii.localhost"

Write-Host "Done!" -ForegroundColor Green
