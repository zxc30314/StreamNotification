FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Stream Notification.csproj", "Stream Notification/"]
RUN dotnet restore "Stream Notification/Stream Notification.csproj"
WORKDIR "/src/Stream Notification"
COPY . .
RUN dotnet build "Stream Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stream Notification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
VOLUME ["/app"]
ENTRYPOINT ["dotnet", "Stream Notification.dll"]
