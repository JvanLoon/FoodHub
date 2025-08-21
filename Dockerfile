# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["FoodCalc.Web/FoodCalc.Web.csproj", "FoodCalc.Web/"]
COPY ["FoodCalc.Api/FoodCalc.Api.csproj", "FoodCalc.Api/"]
COPY . .
RUN dotnet restore "FoodCalc.Web/FoodCalc.Web.csproj"

# Build and publish the app
RUN dotnet publish "FoodCalc.Web/FoodCalc.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:443
ENTRYPOINT ["dotnet", "FoodCalc.Web.dll"]
