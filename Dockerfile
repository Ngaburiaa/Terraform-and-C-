# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY BookRepositoryApi/BookRepositoryApi.csproj BookRepositoryApi/
RUN dotnet restore BookRepositoryApi/BookRepositoryApi.csproj

# Copy everything else and build
COPY BookRepositoryApi/ BookRepositoryApi/
WORKDIR /src/BookRepositoryApi
RUN dotnet build BookRepositoryApi.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish BookRepositoryApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for healthcheck
RUN apt-get update \
  && apt-get install -y --no-install-recommends curl \
  && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BookRepositoryApi.dll"]
