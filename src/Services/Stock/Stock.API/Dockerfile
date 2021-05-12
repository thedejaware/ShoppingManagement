#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Stock/Stock.API/Stock.API.csproj", "Services/Stock/Stock.API/"]
COPY ["Services/Stock/Stock.Application/Stock.Application.csproj", "Services/Stock/Stock.Application/"]
COPY ["Services/Stock/Stock.Domain/Stock.Domain.csproj", "Services/Stock/Stock.Domain/"]
COPY ["Services/Stock/Stock.Infrastructure/Stock.Infrastructure.csproj", "Services/Stock/Stock.Infrastructure/"]
RUN dotnet restore "Services/Stock/Stock.API/Stock.API.csproj"
COPY . .
WORKDIR "/src/Services/Stock/Stock.API"
RUN dotnet build "Stock.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stock.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stock.API.dll"]
