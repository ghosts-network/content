FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY . .
WORKDIR /src/GhostNetwork.Content.Api
RUN dotnet restore GhostNetwork.Content.Api.csproj
RUN dotnet publish GhostNetwork.Content.Api.csproj --no-restore -c Release -o /app

WORKDIR /app
RUN dotnet new tool-manifest --force
RUN dotnet tool install Swashbuckle.AspNetCore.Cli --version 6.4.0
RUN dotnet swagger tofile --output swagger.json GhostNetwork.Content.Api.dll api

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GhostNetwork.Content.Api.dll"]
