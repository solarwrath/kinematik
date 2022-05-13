FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy projects and restore as distinct layers
COPY ["Kinematik-Hosting/Kinematik-Hosting.csproj", "./Kinematik-Hosting/"]
COPY ["Kinematik-HttpApi/Kinematik-HttpApi.csproj", "./Kinematik-HttpApi/"]
COPY ["Kinematik-Application/Kinematik-Application.csproj", "./Kinematik-Application/"]
COPY ["Kinematik-Domain/Kinematik-Domain.csproj", "./Kinematik-Domain/"]
COPY ["Kinematik-EntityFramework/Kinematik-EntityFramework.csproj", "./Kinematik-EntityFramework/"]

RUN dotnet restore "./Kinematik-Hosting"

# Copy everything else and build
COPY . .
RUN dotnet build "./Kinematik-Hosting" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Kinematik-Hosting" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kinematik-Hosting.dll"]