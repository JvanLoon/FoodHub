FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy solution file to allow proper dependency resolution
COPY ["*.sln", "./"]

# Copy all project files first to restore dependencies
COPY ["FoodCalc.Api/*.csproj", "FoodCalc.Api/"]
COPY ["FoodHub.ServiceDefaults/*.csproj", "FoodHub.ServiceDefaults/"]
COPY ["FoodCalc.Features/*.csproj", "FoodCalc.Features/"]
COPY ["FoodHub.Persistence/*.csproj", "FoodHub.Persistence/"]
# Copy any other project references
RUN dotnet restore "FoodCalc.Api/FoodCalc.Api.csproj"

COPY . .
WORKDIR "/src/FoodCalc.Api"
RUN dotnet build "FoodCalc.Api.csproj" -c Release -o /app/build -v normal

FROM build AS publish
RUN dotnet publish "FoodCalc.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodCalc.Api.dll"]
