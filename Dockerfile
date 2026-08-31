FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine

# The suite shells out to these; the SDK image carries neither.
RUN apk add --no-cache bash git

ENV DOTNET_NOLOGO=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true

WORKDIR /src

# Warms the NuGet cache in its own layer, so editing sources does not refetch.
COPY Directory.Build.props Boo.slnx Boo.targets ./
COPY src/ src/
COPY tests/ tests/
RUN dotnet restore Boo.slnx

COPY . .

CMD ["dotnet", "test", "Boo.slnx", "--configuration", "Release"]
