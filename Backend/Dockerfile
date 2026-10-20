FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Backend.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app
COPY --from=build /out ./
USER $APP_UID
ENV ASPNETCORE_URLS=http://0.0.0.0:5156
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5156
ENTRYPOINT ["dotnet", "Backend.dll"]
