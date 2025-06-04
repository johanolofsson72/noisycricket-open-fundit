# Version 5.5

# Steg 1: Definiera basbilden
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN id $APP_UID || (echo "Invalid APP_UID" && exit 1)

# Steg 2: Bygg-steg
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG PROJECT_NAME=Server
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

# Dynamiskt kopiera projektet baserat på bygg-argumentet
COPY ["Solution/src/Common/NuGet.config", "./"]
COPY ["Solution/src/Common/telerik-license.txt", "./"]
COPY ["Solution/src/Common/OpenNLP.dll", "./Solution/src/Shared/TextSummarizer/"]
COPY ["Solution/src/Common/PageRank.dll", "./Solution/src/Shared/TextSummarizer/"]
COPY ["Solution/src/Common/SharpEntropy.dll", "./Solution/src/Shared/TextSummarizer/"]
COPY ["Solution/src/Common/TextRank.dll", "./Solution/src/Shared/TextSummarizer/"]
COPY ["Solution/src/Common/TextSummarizer.dll", "./Solution/src/Shared/TextSummarizer/"]
COPY ["Solution/src/$PROJECT_NAME/$PROJECT_NAME.csproj", "$PROJECT_NAME/"]
COPY ["Solution/src/Shared/Shared.csproj", "Shared/"]

# Återställ projektet
RUN dotnet restore "$PROJECT_NAME/$PROJECT_NAME.csproj"

# Kopiera hela koden
COPY . .

# Felsök: Lista alla filer i /srsc för att säkerställa att lösningen finns dära
#RUN ls -R /app

# Bygg projektet
RUN dotnet build "Solution/src/$PROJECT_NAME/$PROJECT_NAME.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Steg 3: Publicera applikationen
FROM build AS publish
RUN dotnet publish "Solution/src/$PROJECT_NAME/$PROJECT_NAME.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#RUN dotnet publish "Solution/src/$PROJECT_NAME/$PROJECT_NAME.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:PublishTrimmed=true /p:SelfContained=true /p:PublishSingleFile=true /p:UseAppHost=false

# Steg 4: Slutlig bild för att köra applikationen
FROM base AS final
WORKDIR /app

# Skapa data- och templates-kataloger
RUN mkdir -p data
RUN chmod -R a+rwx data/

# Kopiera publicerade filers
COPY --from=publish /app/publish .

# Kör applikationen
ENTRYPOINT ["dotnet", "Server.dll"]
