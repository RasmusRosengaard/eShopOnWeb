# Bygges fra repo-roden (samme context som pipelinen):
# docker build -t catalogservice -f services/CatalogService/CatalogService.Api/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY services/Directory.Packages.props services/
COPY services/CatalogService/ services/CatalogService/
WORKDIR /app/services/CatalogService/CatalogService.Api
RUN dotnet restore
RUN dotnet publish -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/services/CatalogService/CatalogService.Api/out ./
ENTRYPOINT ["dotnet", "CatalogService.Api.dll"]
