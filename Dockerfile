# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first
COPY News_App.sln ./
COPY News_App.Api/News_App.Api.csproj News_App.Api/
COPY News_App.Core/News_App.Core.csproj News_App.Core/
COPY News_App/News_App.csproj News_App/

# Restore dependencies
RUN dotnet restore News_App.sln

# Copy everything else
COPY . .

# Publish the API
RUN dotnet publish News_App.Api/News_App.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# App will listen on this port inside the container
ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "News_App.Api.dll"]