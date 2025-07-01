FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FitFlare.Api/FitFlare.Api.csproj", "FitFlare.Api/"]
COPY ["FitFlare.Application/FitFlare.Application.csproj", "FitFlare.Application/"]
COPY ["FitFlare.Infrastructure/FitFlare.Infrastructure.csproj", "FitFlare.Infrastructure/"]
COPY ["FitFlare.Core/FitFlare.Core.csproj", "FitFlare.Core/"]
RUN dotnet restore "FitFlare.Api/FitFlare.Api.csproj"
COPY . .
WORKDIR "/src/FitFlare.Api"
RUN dotnet build "./FitFlare.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release


RUN dotnet publish "./FitFlare.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitFlare.Api.dll"]
