FROM mcr.microsoft.com/dotnet/sdk:6.0


# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./TechRadarApi.sln ./
COPY ./TechRadarApi/TechRadarApi.csproj ./TechRadarApi/
COPY ./TechRadarApi.Tests/TechRadarApi.Tests.csproj ./TechRadarApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./TechRadarApi/TechRadarApi.csproj
RUN dotnet restore ./TechRadarApi.Tests/TechRadarApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out TechRadarApi/TechRadarApi.csproj
RUN dotnet build -c debug -o out TechRadarApi.Tests/TechRadarApi.Tests.csproj

CMD dotnet test
