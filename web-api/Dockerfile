#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MlffWebApi/MlffWebApi.csproj", "MlffWebApi/"]
RUN dotnet restore "MlffWebApi/MlffWebApi.csproj"
COPY . .
WORKDIR "/src/MlffWebApi"
RUN dotnet build "MlffWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MlffWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MlffWebApi.dll"]
