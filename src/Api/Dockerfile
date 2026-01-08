FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Api/Contasiscorp.Api.csproj", "src/Api/"]
COPY ["src/Application/Contasiscorp.Application.csproj", "src/Application/"]
COPY ["src/Domain/Contasiscorp.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Contasiscorp.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Api/Contasiscorp.Api.csproj"
COPY . .
WORKDIR "/src/src/Api"
RUN dotnet build "Contasiscorp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contasiscorp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contasiscorp.Api.dll"]
