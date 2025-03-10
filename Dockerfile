# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj file and restore dependencies
COPY ["BookManagementSystem.csproj", "./"]
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build the application
RUN dotnet build "BookManagementSystem.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "BookManagementSystem.csproj" -c Release -o /app/publish

# Stage 3: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --gecos "" appuser

# Copy the published app
COPY --from=publish /app/publish .

# Set proper permissions
RUN chown -R appuser:appuser /app
USER appuser

# Configure environment variables (will be overridden at runtime)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "BookManagementSystem.dll"]