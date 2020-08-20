FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY . .
RUN dotnet restore GhostNetwork.Publications.Api/GhostNetwork.Publications.Api.csproj
WORKDIR /src/GhostNetwork.Publications.Api
RUN dotnet build GhostNetwork.Publications.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GhostNetwork.Publications.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GhostNetwork.Publications.Api.dll"]
