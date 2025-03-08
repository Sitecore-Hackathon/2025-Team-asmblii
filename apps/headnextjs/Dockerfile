#escape=`
ARG BUILD_IMAGE=mcr.microsoft.com/windows/servercore:ltsc2022
ARG BASE_IMAGE=mcr.microsoft.com/windows/servercore:ltsc2022

FROM ${BUILD_IMAGE} AS download
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ARG NODEJS_VERSION=v23.6.1
RUN $Uri = 'https://nodejs.org/dist/{0}/node-{0}-win-x64.zip' -f $env:NODEJS_VERSION; `
    $OutFile = 'c:/node-v{0}-win-x64.zip' -f $env:NODEJS_VERSION ; `
    Invoke-WebRequest -Uri $Uri -OutFile $OutFile -Verbose;  `
    Expand-Archive $OutFile -DestinationPath c:/ ; `
    Copy-Item $('c:/node-{0}-win-x64' -f $env:NODEJS_VERSION) -Destination c:/out/nodejs -Recurse ; `
    Remove-Item $('c:/node-{0}-win-x64' -f $env:NODEJS_VERSION) -Recurse -Force ;


FROM ${BASE_IMAGE} AS nodejs

RUN md %APPDATA%\npm
ENV PATH=C:\Windows\system32;C:\Windows;c:\nodejs;%APPDATA%\npm 
COPY --from=download /out/ /

CMD [ "node.exe" ]


FROM nodejs AS deps
WORKDIR /app

# Install dependencies based on the preferred package manager
COPY package.json package-lock.json ./
RUN npm ci

FROM deps AS builder
WORKDIR /app
COPY . .

ENV NEXT_TELEMETRY_DISABLED=1
ARG SITECORE_EDGE_URL="" `
    SITECORE_EDGE_CONTEXT="" `
      SITECORE_API_KEY="" `
      SITECORE_API_HOST="" `
      PUBLIC_URL="" `
      OTEL_EXPORTER_OTLP_TRACES_ENDPOINT="" `
      OTEL_EXPORTER_OTLP_METRICS_ENDPOINT="" `
      NEXT_TELEMETRY_DISABLED="1"
RUN npm run build


FROM deps AS runner
WORKDIR /app

#COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static


EXPOSE 3000
ENV PORT=3000

# server.js is created by next build from the standalone output
# https://nextjs.org/docs/pages/api-reference/config/next-config-js/output
ENV HOSTNAME="0.0.0.0"
CMD ["node", "server.js", "--require", "instrumentation.node.js"]
