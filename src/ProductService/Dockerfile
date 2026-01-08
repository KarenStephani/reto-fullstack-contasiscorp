FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/ProductService/ProductService.csproj", "src/ProductService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

RUN dotnet restore "src/ProductService/ProductService.csproj"

COPY . .

RUN dotnet build "src/ProductService/ProductService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/ProductService/ProductService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ProductService.dll"]
