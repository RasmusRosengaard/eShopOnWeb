# Bygges fra repo-roden (samme context som pipelinen):
# docker build -t gateway -f services/Gateway/Gateway.Api/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY services/Directory.Packages.props services/
COPY services/Gateway/Gateway.Api/ services/Gateway/Gateway.Api/
WORKDIR /app/services/Gateway/Gateway.Api
RUN dotnet restore
RUN dotnet publish -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/services/Gateway/Gateway.Api/out ./
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
