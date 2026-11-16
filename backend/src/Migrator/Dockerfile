# Build stage: publish the migration runner and ship the SQL migrations beside it.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Directory.Build.props ./
COPY backend/src/ ./backend/src/

RUN dotnet publish "backend/src/Migrator/BallastLane.Migrator.csproj" --configuration Release --output /app

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

# Npgsql optionally uses GSSAPI; without this the runtime logs a missing-library error.
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app ./
COPY backend/db/migrations ./db/migrations
ENTRYPOINT ["dotnet", "BallastLane.Migrator.dll"]
