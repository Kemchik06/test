# src/user/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /app

COPY . .

RUN dotnet restore UserAuth.Api/UserAuth.Api.csproj

RUN dotnet publish UserAuth.Api/UserAuth.Api.csproj -c Release -o /published --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=build /published ./

RUN adduser -D -H appuser && chown -R appuser /app
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "UserAuth.Api.dll"]