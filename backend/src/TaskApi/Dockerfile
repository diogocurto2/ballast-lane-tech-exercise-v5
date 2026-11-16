# Build stage: publish the task host from the shared solution.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Directory.Build.props ./
COPY backend/src/ ./backend/src/

RUN dotnet publish "backend/src/TaskApi/BallastLane.TaskApi.csproj" --configuration Release --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Npgsql optionally uses GSSAPI; without this the runtime logs a missing-library error.
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BallastLane.TaskApi.dll"]
