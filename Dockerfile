#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5002
EXPOSE 5001
EXPOSE 5180
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001;http://+:5180


FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["HelloAssetAdministrationShell/HelloAssetAdministrationShell.csproj", "HelloAssetAdministrationShell/"]
RUN dotnet restore "HelloAssetAdministrationShell/HelloAssetAdministrationShell.csproj"
COPY . .
WORKDIR "/src/HelloAssetAdministrationShell"
RUN dotnet build "HelloAssetAdministrationShell.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelloAssetAdministrationShell.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV AASSERVERADDRESS="http://localhost:5180"
ENV MQTTSOUTHBOUND_BROKERADDRESS="test.mosquitto.org"
ENV MQTTSOUTHBOUND_PORT=1883
ENV MQTTSOUTHBOUND_SUBSCRIPTIONTOPIC="DMU80eVo"

ENV MQTTNORTHBOUND_BROKERADDRESS="test.mosquitto.org"
ENV MQTTNORTHBOUND_PORT=1883
ENV MQTTNORTHBOUND_SUBSCRIPTIONTOPIC="aas-notification"
ENV MQTTNORTHBOUND_PUBLICATIONTOPIC="BasyxMesAASOrderHandling"

ENTRYPOINT ["dotnet", "HelloAssetAdministrationShell.dll"]