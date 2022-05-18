FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy projects and restore as distinct layers
COPY ["Kinematik.Hosting/Kinematik.Hosting.csproj", "./Kinematik.Hosting/Kinematik.Hosting.csproj"]
COPY ["Kinematik.HttpApi/Kinematik.HttpApi.csproj", "./Kinematik.HttpApi/Kinematik.HttpApi.csproj"]
COPY ["Kinematik.Application/Kinematik.Application.csproj", "./Kinematik.Application/Kinematik.Application.csproj"]
COPY ["Kinematik.Domain/Kinematik.Domain.csproj", "./Kinematik.Domain/Kinematik.Domain.csproj"]
COPY ["Kinematik.EntityFramework/Kinematik.EntityFramework.csproj", "./Kinematik.EntityFramework/Kinematik.EntityFramework.csproj"]
COPY ["Kinematik.FileStorage/Kinematik.FileStorage.csproj", "./Kinematik.FileStorage/Kinematik.FileStorage.csproj"]

RUN dotnet restore "./Kinematik.Hosting/Kinematik.Hosting.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "./Kinematik.Hosting/Kinematik.Hosting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Kinematik.Hosting/Kinematik.Hosting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kinematik.Hosting.dll"]

VOLUME "./FileUploads/"