#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ARG DEBIAN_FRONTEND=noninteractive
ENV TZ=Europe/Bern

RUN apt-get update && apt-get install -y \
    git \
    libhidapi-dev \
    build-essential \
    gettext

RUN git clone https://github.com/flok99/clewarecontrol.git && cd clewarecontrol && make install

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster-arm32v7 AS build
WORKDIR /src

# Prevent 'Warning: apt-key output should not be parsed (stdout is not a terminal)'
ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=1

# install NodeJS
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install -y nodejs

COPY ["busylight.csproj", ""]
RUN dotnet restore "./busylight.csproj"
COPY . .
WORKDIR "/src/."

RUN dotnet build "busylight.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "busylight.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "busylight.dll"]
