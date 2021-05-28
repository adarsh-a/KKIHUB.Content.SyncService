FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 44381


FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["KKIHUB.Content.SyncService.csproj", ""]
RUN dotnet restore "./KKIHUB.Content.SyncService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "KKIHUB.Content.SyncService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KKIHUB.Content.SyncService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KKIHUB.Content.SyncService.dll"]