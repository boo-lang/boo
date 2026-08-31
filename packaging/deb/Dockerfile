FROM mcr.microsoft.com/dotnet/sdk:10.0-noble

RUN apt-get update \
 && apt-get install -y --no-install-recommends dpkg-dev \
 && rm -rf /var/lib/apt/lists/*

ENV DOTNET_NOLOGO=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true

WORKDIR /src
COPY . .

ENTRYPOINT ["/src/packaging/deb/build.sh"]
