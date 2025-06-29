FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080
EXPOSE 8081
EXPOSE 5000
EXPOSE 5001

# Copy project files (paths are relative to root context)
COPY ["src/IdentityServer/IdentityServer.csproj", "src/IdentityServer/"]
COPY ["src/TaskManager.SharedKernel/TaskManager.SharedKernel.csproj", "src/TaskManager.SharedKernel/"]

# Restore packages
RUN dotnet restore "src/IdentityServer/IdentityServer.csproj"

# Copy remaining files
COPY . .

# Build and publish
WORKDIR "/src/src/IdentityServer"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]