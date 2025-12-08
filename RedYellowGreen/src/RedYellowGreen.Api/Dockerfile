FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# postgres healthcheck script
COPY wait-for-it.sh /app/wait-for-it.sh
RUN chmod +x /app/wait-for-it.sh

USER $APP_UID
WORKDIR /app
EXPOSE 5000


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/RedYellowGreen.Api/RedYellowGreen.Api.csproj", "src/RedYellowGreen.Api/"]
RUN dotnet restore "src/RedYellowGreen.Api/RedYellowGreen.Api.csproj"
COPY . .
WORKDIR "/src/src/RedYellowGreen.Api"
RUN dotnet build "./RedYellowGreen.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RedYellowGreen.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/bin/sh", "-c", "echo 'Starting wait-for-it'; /app/wait-for-it.sh database:5432 -- echo 'Database is ready, starting app...' && dotnet RedYellowGreen.Api.dll"]

