FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY . .
RUN dotnet restore GhostNetwork.Reactions.Api/GhostNetwork.Reactions.Api.csproj
WORKDIR /src/GhostNetwork.Reactions.Api
RUN dotnet build GhostNetwork.Reactions.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GhostNetwork.Reactions.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GhostNetwork.Reactions.Api.dll"]
