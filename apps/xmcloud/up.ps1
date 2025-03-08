param(
    [Parameter(Mandatory = $false)]
    [switch]$RebuildIndexes
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# Set the root of the repository
$repoRoot = Resolve-Path "$PSScriptRoot/."

# Parse .env values
$envFileLocation = "$repoRoot/.env"
$envContent = Get-Content $envFileLocation -Encoding UTF8
$xmCloudHost = $envContent | Where-Object { $_ -imatch "^CM_HOST=.+" } | ForEach-Object { $_.Substring($_.IndexOf("=") + 1) }
$licenseFolder = $envContent | Where-Object { $_ -imatch "^HOST_LICENSE_FOLDER=.+" } | ForEach-Object { $_.Substring($_.IndexOf("=") + 1) }

# Check license
$licenseXmlPath = Join-Path $licenseFolder "license.xml"

if (-not (Test-Path $licenseXmlPath))
{
    throw "License not found at '$licenseXmlPath'."
}

$licenseXml = [xml](Get-Content $licenseXmlPath)
$licenseExpiration = $licenseXml.SelectNodes("//expiration")[0].InnerText
$licenseExpirationDate = [System.DateTime]::ParseExact($licenseExpiration, "yyyyMMddThhmmss", [System.Globalization.CultureInfo]::InvariantCulture)

if ($licenseExpirationDate -lt (Get-Date))
{
    throw "Your license has expired at '$licenseExpirationDate', please update your license file at '$licenseXmlPath'."
}
else
{
    Write-Host "The license is valid. $(($licenseExpirationDate - (Get-Date)).Days) days left until expiration." -ForegroundColor Green
}

# Update images
Write-Host "Keeping all images up to date..." -ForegroundColor Green
(docker compose config | Select-String "(scr\.sitecore\.com\/.+)|(mcr\.microsoft\.com\/.+)|(traefik\:v.+)").Matches | ForEach-Object { $_.Value } | ForEach-Object { docker image pull $_ }

# Build all services
Write-Host "Building containers..." -ForegroundColor Green
docker compose build
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Container build failed, see errors above."
}

# Start the Sitecore instance
Write-Host "Starting Sitecore environment..." -ForegroundColor Green
docker compose up -d

# Wait for Traefik to expose CM route
Write-Host "Waiting for CM to become available..." -ForegroundColor Green -NoNewline
$startTime = Get-Date
do
{
    Start-Sleep -Milliseconds 100
    try
    {
        $status = Invoke-RestMethod "http://localhost:8079/api/http/routers/cm-secure@docker"
    }
    catch
    {
        if ($_.Exception.Response.StatusCode.value__ -ne "404")
        {
            throw
        }
    }
    Write-Host "." -ForegroundColor Green -NoNewline
} while ($status.status -ne "enabled" -and $startTime.AddSeconds(300) -gt (Get-Date))

Write-Host "`n" -NoNewline

if (-not $status.status -eq "enabled")
{
    $status

    Write-Error "Timeout waiting for Sitecore CM to become available via Traefik proxy. Check CM container logs."
}

# Install Sitecore CLI
Write-Host "Restoring Sitecore CLI..." -ForegroundColor Green
dotnet tool restore
Write-Host "Installing Sitecore CLI Plugins..."
dotnet sitecore --help | Out-Null
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Unexpected error installing Sitecore CLI Plugins"
}

# Login
Write-Host "Logging into Sitecore..." -ForegroundColor Green
dotnet sitecore cloud login
dotnet sitecore connect --ref xmcloud --cm https://$xmCloudHost --allow-write true -n default
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Unable to log into Sitecore, did the Sitecore environment start correctly? See logs above."
}

if ($RebuildIndexes)
{
    # Populate Solr managed schemas to avoid errors during item deploy
    Write-Host "Populating Solr managed schema..." -ForegroundColor Green
    dotnet sitecore index schema-populate
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error "Populating Solr managed schema failed, see errors above."
    }

    # Rebuild indexes
    Write-Host "Rebuilding indexes ..." -ForegroundColor Green
    dotnet sitecore index rebuild
}

# Push serialized items
Write-Host "Pushing items..." -ForegroundColor Green
dotnet sitecore ser push

# Done
Write-Host "Sitecore ready! Check https://$xmCloudHost/sitecore/" -ForegroundColor Green
