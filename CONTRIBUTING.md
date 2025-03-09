# Contributing

...

## Running locally

Run once:

1. `.\init.ps1`
1. `.\up.ps1 -RebuildIndexes`

After that you can work normally:

1. `.\up.ps1` (or `docker compose up -d --build` and `Push-Location .\apps\xmcloud; dotnet sitecore ser push; Pop-Location`)
1. Example, rebuild and restart single app: `docker compose up -d api-app --build`

Shutdown:

1. `docker compose down`

## Links

- XM Cloud: <https://xmcloudcm.localhost/sitecore/>
- Traefik: <http://localhost:8079>
- Aspire Dashboard: <https://dashboard.2025-team-asmblii.localhost>
- API app: <https://api-app.2025-team-asmblii.localhost>
- API solr: <https://api-solr.2025-team-asmblii.localhost/solr/>
- NextJS head: <https://headnextjss.2025-team-asmblii.localhost>
