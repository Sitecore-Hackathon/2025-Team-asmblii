![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

# Sitecore Hackathon 2025

- MUST READ: **[Submission requirements](SUBMISSION_REQUIREMENTS.md)**
- [Entry form template](ENTRYFORM.md)

## Team name

Team asmblii

## Category

...

## Description

### Effective observability of composable DXP's using OpenTelemetry

OpenTelemetry (OTel) is an open-source, [CNCF backed](https://www.cncf.io/projects/opentelemetry/), cross-platform, vendor-neutral framework and specification for making systems observable. It achieves this by instrumenting code to emit traces, metrics, and logs in a standardized format, which can then be sent to an observability backend.

Here we use the [ASP.NET Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash) as it is a simple OTel collector and UI that is easy to run locally. Alternatives could be full-fledged platforms such as SigNoz, New Relic, DataDog, Azure App Insights etc. which all supports ingesting OTel data.

TODO: insert image of aspire with a deep trace

The demo architecture:

![Architecture](docs/images/architecture.png?raw=true "Architecture")

What parts do we instrument in this submission?

1. Traefik (OTel built in)
    - Traces
    - Metrics
    - Logs
1. Next.js/JSS Head (uses OTel JavaScript SDK)
    - Traces (calls to XM Cloud GraphQL endpoint, calls to the ASP.NET Core API)
    - Metrics
1. ASP.NET Core API (uses OTel .NET SDK)
    - Traces (HTTP calls to external API's, calls to Solr)
    - Metrics
    - Logs
1. Solr (uses OTel zero-code instrumentation)
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

1. Browse <https://headnextjss.2025-team-asmblii.localhost/>...
1. Open <https://dashboard.2025-team-asmblii.localhost> explore all the glorious traces, metrics and structured logs 🦄🎉!
1. You can also try these urls to force interesting data:
    - <https://dashboard.2025-team-asmblii.localhost/slow> (trace with high duration)
    - <https://dashboard.2025-team-asmblii.localhost/throw> (trace that has errors)

## Comments

...
