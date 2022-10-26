FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY  ["src/", "."]
#COPY ["src/ThreeThings.Api/ThreeThings.Api.csproj", "ThreeThings.Api/"]
RUN dotnet restore "ThreeThings.Api/ThreeThings.Api.csproj"
COPY . .
WORKDIR "/src/ThreeThings.Api"
RUN dotnet build "ThreeThings.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ThreeThings.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreeThings.Api.dll"]
