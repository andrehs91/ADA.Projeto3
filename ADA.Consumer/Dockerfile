#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app
ENV TZ="America/Sao_Paulo"
ARG CONNECTIONSTRINGS_REDIS
ARG RABBITMQ_HOSTNAME
ARG RABBITMQ_USERNAME
ARG RABBITMQ_PASSWORD
ENV CONNECTIONSTRINGS_REDIS=${CONNECTIONSTRINGS_REDIS}
ENV RABBITMQ_HOSTNAME=${RABBITMQ_HOSTNAME}
ENV RABBITMQ_USERNAME=${RABBITMQ_USERNAME}
ENV RABBITMQ_PASSWORD=${RABBITMQ_PASSWORD}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ADA.Consumer/ADA.Consumer.csproj", "ADA.Consumer/"]
RUN dotnet restore "./ADA.Consumer/./ADA.Consumer.csproj"
COPY . .
WORKDIR "/src/ADA.Consumer"
RUN dotnet build "./ADA.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ADA.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ADA.Consumer.dll"]