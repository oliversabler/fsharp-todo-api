FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/api/Web.Api.fsproj", "api/"]
RUN dotnet restore "api/Web.Api.fsproj"
COPY . .
RUN dotnet build "src/api/Web.Api.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/api/Web.Api.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.Api.dll"]
