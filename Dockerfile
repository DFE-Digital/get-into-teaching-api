# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /source
ARG GIT_COMMIT_SHA
ENV ASPNETCORE_URLS=http://+:8080

# copy csproj and restore as distinct layers
COPY *.sln .
COPY GetIntoTeachingApi/*.csproj ./GetIntoTeachingApi/
COPY GetIntoTeachingApiTests/*.csproj ./GetIntoTeachingApiTests/
RUN dotnet restore -r linux-x64

# copy everything else and build app
COPY GetIntoTeachingApi/. ./GetIntoTeachingApi/
WORKDIR /source/GetIntoTeachingApi
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "GetIntoTeachingApi.dll"]
ENV ASPNETCORE_URLS=http://+:8080
ARG GIT_COMMIT_SHA
ENV GIT_COMMIT_SHA ${GIT_COMMIT_SHA}
