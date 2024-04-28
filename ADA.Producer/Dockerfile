#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV TZ="America/Sao_Paulo"
ARG CONNECTIONSTRINGS_REDIS
ARG RABBITMQ_HOSTNAME
ARG RABBITMQ_USERNAME
ARG RABBITMQ_PASSWORD
ARG MINIO_ENDPOINT
ARG MINIO_ACCESSKEY
ARG MINIO_SECRETKEY
ARG MINIO_ISSECURE
ARG MINIO_PORT
ENV CONNECTIONSTRINGS_REDIS=${CONNECTIONSTRINGS_REDIS}
ENV RABBITMQ_HOSTNAME=${RABBITMQ_HOSTNAME}
ENV RABBITMQ_USERNAME=${RABBITMQ_USERNAME}
ENV RABBITMQ_PASSWORD=${RABBITMQ_PASSWORD}
ENV MINIO_ENDPOINT=${MINIO_ENDPOINT}
ENV MINIO_ACCESSKEY=${MINIO_ACCESSKEY}
ENV MINIO_SECRETKEY=${MINIO_SECRETKEY}
ENV MINIO_ISSECURE=${MINIO_ISSECURE}
ENV MINIO_PORT=${MINIO_PORT}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ADA.Producer/ADA.Producer.csproj", "ADA.Producer/"]
RUN dotnet restore "./ADA.Producer/./ADA.Producer.csproj"
COPY . .
WORKDIR "/src/ADA.Producer"
RUN dotnet build "./ADA.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ADA.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ADA.Producer.dll"]