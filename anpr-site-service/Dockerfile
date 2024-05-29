FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MlffSiteService/MlffSiteService.csproj", "MlffSiteService/"]
RUN dotnet restore "MlffSiteService/MlffSiteService.csproj"
COPY . .
WORKDIR "/src/MlffSiteService"
RUN dotnet build "MlffSiteService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MlffSiteService.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt update
RUN apt install cifs-utils -y
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MlffSiteService.dll"]