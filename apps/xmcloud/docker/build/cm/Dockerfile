# escape=`

ARG PARENT_IMAGE
ARG TOOLS_IMAGE

FROM ${TOOLS_IMAGE} as tools
FROM ${PARENT_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR C:\inetpub\wwwroot

COPY --from=tools C:\tools C:\tools

# needed since we do not mount host folder
RUN New-Item C:\deploy -ItemType Directory | Out-Null
