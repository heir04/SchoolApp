# Use the official .NET 9.0 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["schoolapp.csproj", "."]
RUN dotnet restore "schoolapp.csproj"

# Copy the entire source code
COPY . .

# Build the application
RUN dotnet build "schoolapp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "schoolapp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for data protection keys
RUN mkdir -p /app/keys && chmod 700 /app/keys

# Set the entry point
ENTRYPOINT ["dotnet", "schoolapp.dll"]
