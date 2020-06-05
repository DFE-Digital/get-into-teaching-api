# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
ENV ASPNETCORE_URLS=http://+:8080

# copy csproj and restore as distinct layers
COPY *.sln .
COPY GetIntoTeachingApi/*.csproj ./GetIntoTeachingApi/
COPY GetIntoTeachingApiTests/*.csproj ./GetIntoTeachingApiTests/
RUN dotnet nuget add source --name NotifyBintray https://api.bintray.com/nuget/gov-uk-notify/nuget
RUN dotnet restore -r linux-x64 

# copy everything else and build app
COPY GetIntoTeachingApi/. ./GetIntoTeachingApi/
WORKDIR /source/GetIntoTeachingApi
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# Install dependencies
RUN apt-get update && apt-get install -y \
	libsqlite3-mod-spatialite \
&& rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "GetIntoTeachingApi.dll"]
ENV ASPNETCORE_URLS=http://+:8080
