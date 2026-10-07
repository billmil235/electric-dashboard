FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS api-base
USER $APP_UID
WORKDIR /app
EXPOSE 5281

FROM node:22-alpine AS frontend-build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build -- --configuration production

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS api-build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElectricDashboard.Api/ElectricDashboard.Api.csproj", "ElectricDashboard.Api/"]
RUN dotnet restore "ElectricDashboard.Api/ElectricDashboard.Api.csproj"
COPY . .
WORKDIR "/src/ElectricDashboard.Api"
RUN dotnet build "./ElectricDashboard.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM api-build AS api-publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ElectricDashboard.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM api-base AS api-final
WORKDIR /app
COPY --from=api-publish /app/publish .
COPY --from=frontend-build /app/dist/ElectricDashboard/browser ./wwwroot
ENTRYPOINT ["dotnet", "ElectricDashboard.Api.dll"]
