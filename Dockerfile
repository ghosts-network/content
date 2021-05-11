FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore GhostNetwork.Content.Api/GhostNetwork.Content.Api.csproj
WORKDIR /src/GhostNetwork.Content.Api
RUN dotnet build GhostNetwork.Content.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GhostNetwork.Content.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GhostNetwork.Content.Api.dll"]
