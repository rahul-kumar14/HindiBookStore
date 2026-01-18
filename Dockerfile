# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published app
COPY --from=build /app .

# Create uploads directory for persistent storage
RUN mkdir -p /app/wwwroot/uploads/images && \
    mkdir -p /app/wwwroot/uploads/pdfs && \
        chmod -R 777 /app/wwwroot/uploads

        # Set environment variables
        ENV ASPNETCORE_URLS=http://+:8080
        ENV ASPNETCORE_ENVIRONMENT=Production

        EXPOSE 8080

        ENTRYPOINT ["dotnet", "HindiBookStore.dll"]
        
