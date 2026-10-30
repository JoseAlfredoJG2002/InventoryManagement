
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Inventory.Api/Inventory.Api.csproj", "Inventory.Api/"]
COPY ["Inventory.Application/Inventory.Application.csproj", "Inventory.Application/"]
COPY ["Inventory.Domain/Inventory.Domain.csproj", "Inventory.Domain/"]
COPY ["Inventory.Infrastructure/Inventory.Infrastructure.csproj", "Inventory.Infrastructure/"]
RUN dotnet restore "Inventory.Api/Inventory.Api.csproj"
COPY . .
WORKDIR /src/Inventory.Api
RUN dotnet publish "Inventory.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]
