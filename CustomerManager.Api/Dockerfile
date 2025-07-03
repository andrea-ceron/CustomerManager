# Vedere https://aka.ms/customizecontainer per informazioni su come personalizzare il contenitore di debug e su come Visual Studio usa questo Dockerfile per compilare le immagini per un debug più rapido.

# Questa fase viene usata durante l'esecuzione da Visual Studio in modalità rapida (impostazione predefinita per la configurazione di debug)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Questa fase viene usata per compilare il progetto di servizio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["CustomerManager.Api/CustomerManager.Api.csproj", "CustomerManager.Api/"]
COPY ["CustomerManager.Business/CustomerManager.Business.csproj", "CustomerManager.Business/"]
COPY ["CustomerManager.Repository/CustomerManager.Repository.csproj", "CustomerManager.Repository/"]
COPY ["CustomerManager.Shared/CustomerManager.Shared.csproj", "CustomerManager.Shared/"]
RUN dotnet restore "./CustomerManager.Api/CustomerManager.Api.csproj"
COPY . .
WORKDIR "/src/CustomerManager.Api"
RUN dotnet build "./CustomerManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Questa fase viene usata per pubblicare il progetto di servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CustomerManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Questa fase viene usata nell'ambiente di produzione o durante l'esecuzione da Visual Studio in modalità normale (impostazione predefinita quando non si usa la configurazione di debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerManager.Api.dll"]