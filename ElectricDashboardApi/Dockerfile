FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElectricDashboardApi/ElectricDashboardApi.csproj", "ElectricDashboardApi/"]
RUN dotnet restore "ElectricDashboardApi/ElectricDashboardApi.csproj"
COPY . .
WORKDIR "/src/ElectricDashboardApi"
RUN dotnet build "./ElectricDashboardApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ElectricDashboardApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElectricDashboardApi.dll"]
