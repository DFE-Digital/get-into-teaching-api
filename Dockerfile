# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source
ARG GIT_COMMIT_SHA
ENV ASPNETCORE_URLS=http://+:8080

# copy csproj and restore as distinct layers
COPY *.sln .
COPY GetIntoTeachingApi/*.csproj ./GetIntoTeachingApi/
COPY GetIntoTeachingApiTests/*.csproj ./GetIntoTeachingApiTests/

# copy everything else and build app
COPY GetIntoTeachingApi/. ./GetIntoTeachingApi/
WORKDIR /source/GetIntoTeachingApi
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

# Create a non-root user
RUN addgroup -S appgroup -g 20001 && adduser -S appuser -G appgroup -u 10001

WORKDIR /app
COPY --from=build /app ./

# Switch to non-root user
USER 10001

ENTRYPOINT ["dotnet", "GetIntoTeachingApi.dll"]
ENV ASPNETCORE_URLS=http://+:8080
ARG COMMIT_SHA
ENV GIT_COMMIT_SHA=${COMMIT_SHA}
