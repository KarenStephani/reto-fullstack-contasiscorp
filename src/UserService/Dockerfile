FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/UserService/UserService.csproj", "src/UserService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

RUN dotnet restore "src/UserService/UserService.csproj"

COPY . .

RUN dotnet build "src/UserService/UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/UserService/UserService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "UserService.dll"]
