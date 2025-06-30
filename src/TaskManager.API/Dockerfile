# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files first (better layer caching)
COPY ["src/TaskManager.API/TaskManager.API.csproj", "TaskManager.API/"]
COPY ["src/TaskManager.Application/TaskManager.Application.csproj", "TaskManager.Application/"]
COPY ["src/TaskManager.Domain/TaskManager.Domain.csproj", "TaskManager.Domain/"]
COPY ["src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "TaskManager.Infrastructure/"]
COPY ["src/TaskManager.SharedKernel/TaskManager.SharedKernel.csproj", "TaskManager.SharedKernel/"]

# Restore all projects
RUN dotnet restore "TaskManager.API/TaskManager.API.csproj"

# Copy everything else
COPY . .

# Build the API project
WORKDIR "/src/TaskManager.API"
RUN dotnet build "TaskManager.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TaskManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]