FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/AuthService/AuthService.csproj", "src/AuthService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

RUN dotnet restore "src/AuthService/AuthService.csproj"

COPY . .

RUN dotnet build "src/AuthService/AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/AuthService/AuthService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AuthService.dll"]
