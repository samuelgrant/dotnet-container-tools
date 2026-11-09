FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /src

RUN apt-get update && apt-get install -y --no-install-recommends \
    clang \
    zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

COPY healthcheck.csproj ./healthcheck/
RUN dotnet restore healthcheck/healthcheck.csproj

COPY . ./healthcheck/

RUN dotnet publish healthcheck/healthcheck.csproj \
    -c Release \
    -r linux-x64 \
    -o /publish \
    --no-restore \
    -p:PublishAot=true \
    -p:PublishTrimmed=true