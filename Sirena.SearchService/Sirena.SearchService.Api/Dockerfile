#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sirena.SearchService.Api/Sirena.SearchService.Api.csproj", "Sirena.SearchService.Api/"]
COPY ["Sirena.SearchService.Application.Contracts/Sirena.SearchService.Application.Contracts.csproj", "Sirena.SearchService.Application.Contracts/"]
RUN dotnet restore "Sirena.SearchService.Api/Sirena.SearchService.Api.csproj"
COPY . .
WORKDIR "/src/Sirena.SearchService.Api"
RUN dotnet build "Sirena.SearchService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sirena.SearchService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sirena.SearchService.Api.dll"]