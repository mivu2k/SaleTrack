FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SaleTrack.Core/SaleTrack.Core.csproj SaleTrack.Core/
COPY SaleTrack.Data/SaleTrack.Data.csproj SaleTrack.Data/
COPY SaleTrack.Web/SaleTrack.Web.csproj SaleTrack.Web/
RUN dotnet restore SaleTrack.Web/SaleTrack.Web.csproj

COPY . .
RUN dotnet publish SaleTrack.Web/SaleTrack.Web.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
VOLUME ["/data"]

ENTRYPOINT ["dotnet", "SaleTrack.Web.dll"]
