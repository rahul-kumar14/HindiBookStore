# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published app
COPY --from=build /app ./

# Create uploads directory for persistent storage
RUN mkdir -p wwwroot/uploads/images && \
    mkdir -p wwwroot/uploads/pdfs && \
    chmod -R 777 wwwroot/uploads

# Set environment variables
# Render uses PORT environment variable, default to 10000
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production

# Render expects port 10000 by default
EXPOSE ${PORT:-10000}

ENTRYPOINT ["dotnet", "HindiBookStore.dll"]
