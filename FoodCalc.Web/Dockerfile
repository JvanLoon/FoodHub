FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FoodCalc.Web/FoodCalc.Web.csproj", "FoodCalc.Web/"]
COPY ["FoodHub.ServiceDefaults/FoodHub.ServiceDefaults.csproj", "FoodHub.ServiceDefaults/"]
# Copy any other project references
RUN dotnet restore "FoodCalc.Web/FoodCalc.Web.csproj"

COPY . .
WORKDIR "/src/FoodCalc.Web"
RUN dotnet build "FoodCalc.Web.csproj" -c Release -o /app/build -v normal

FROM build AS publish
RUN dotnet publish "FoodCalc.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodCalc.Web.dll"]
