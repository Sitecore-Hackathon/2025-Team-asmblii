param(
    [Parameter(Mandatory = $false)]
    [switch]$RebuildIndexes
)

# ensure Windows docker engine is running
docker desktop engine use windows

# ensure images are up to date
Write-Host "Keeping images up to date..." -ForegroundColor Green
(docker compose config | Select-String "(scr\.sitecore\.com\/.+)|(mcr\.microsoft\.com\/.+)|(traefik\:v.+)").Matches | ForEach-Object { $_.Value } | ForEach-Object { docker image pull $_ }

# build stack
Write-Host "Build stack..." -ForegroundColor Green

docker compose build

if ($LASTEXITCODE -ne 0)
{
  Write-Error "Container build failed, see errors above."
}

# start stack
Write-Host "Starting stack..." -ForegroundColor Green

docker compose up -d

# start local XM Cloud instance
Push-Location .\apps\xmcloud

try
{
  .\up.ps1 -RebuildIndexes:$RebuildIndexes
}
finally
{
  Pop-Location
}

# finish
Write-Host "Done!" -ForegroundColor Green