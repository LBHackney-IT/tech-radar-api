FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./TechRadarApi.sln ./
COPY ./TechRadarApi/TechRadarApi.csproj ./TechRadarApi/
COPY ./TechRadarApi.Tests/TechRadarApi.Tests.csproj ./TechRadarApi.Tests/

RUN dotnet restore ./TechRadarApi/TechRadarApi.csproj
RUN dotnet restore ./TechRadarApi.Tests/TechRadarApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out TechRadarApi.Tests/TechRadarApi.Tests.csproj

CMD dotnet test
