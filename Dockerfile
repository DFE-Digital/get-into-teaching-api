# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
ARG GIT_COMMIT_SHA
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

# Upgrade the distrubution to clear CVE warning
# and install .net core SDK
RUN apt-get update -y && \
	apt-get dist-upgrade -y && \
	apt-get install --no-install-recommends wget=1.20.1-1.1 -y && \
	wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
	dpkg -i packages-microsoft-prod.deb && \
	apt-get install -y --no-install-recommends apt-transport-https=1.8.2.1 && \
	apt-get update && \
	apt-get install -y --no-install-recommends dotnet-sdk-3.1=3.1.403-1 && \
    apt-get clean && \
	rm -rf /var/lib/apt/lists/*

# Install dotnet-ef for running migrations
RUN dotnet tool install --global dotnet-ef

WORKDIR /app
COPY --from=build /app ./
COPY entrypoint.sh ./entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]
ENV ASPNETCORE_URLS=http://+:8080
ARG GIT_COMMIT_SHA
ENV GIT_COMMIT_SHA ${GIT_COMMIT_SHA}