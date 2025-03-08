![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

# Sitecore Hackathon 2025

- MUST READ: **[Submission requirements](SUBMISSION_REQUIREMENTS.md)**
- [Entry form template](ENTRYFORM.md)

## Team name

⟹ Team asmblii

## Category

⟹ All of 'em!

## Description

### Effective observability of composable DXP's using OpenTelemetry

OpenTelemetry (OTel) is an open-source, [CNCF backed](https://www.cncf.io/projects/opentelemetry/), cross-platform, vendor-neutral framework and specification for making systems observable. It achieves this by instrumenting code to emit traces, metrics, and logs in a standardized format, which can then be sent to an observability backend.

In this case the [ASP.NET Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash) as used as it is a simple OTel collector and UI that is easy to run locally. Alternatives could be SigNoz, New Relic, DataDog, Azure App Insights etc. which all supports ingesting OTel data.

The demo architecture:

````text

client -> traefik - next.js -> asp.net api -> solr

````


What parts of the stack do we instrument in this submission?

1. Traefik
    - Traces
    - Metrics
    - Logs
1. Next.js/JSS
    - Traces
    - Metrics
1. ASP.NET Core API
    - Traces
    - Metrics
    - Logs
1. Solr
    - Traces
    - Metrics
    - Logs


## Video link

⟹ [Replace this Video link](#video-link)

## Pre-requisites and Dependencies

- Windows 11 24H2 or later
- Docker Desktop >= **v4.39.0**
- Valid Sitecore license at `C:\license\license.xml`

## Installation instructions

1. `.\init.ps1`
1. `.\up.ps1 -RebuildIndexes`

### Configuration

No special or manual configuration needed! 🚀🚀🚀

## Usage instructions

1. Browse <https://headnextjss.2025-team-asmblii.localhost/>, refresh a few times...
1. Open <https://dashboard.2025-team-asmblii.localhost> explore all the glorious traces, metrics and structured logs 🦄🎉!
1. TODO: browse url that calls api-app endpoint that fails with 500, tell how to find in dashboard
1. TODO: browse url that calls api-app endpoint that is VERY slow, tell how to find in dashboard

## Comments

TODO
