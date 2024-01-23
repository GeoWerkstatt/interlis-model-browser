FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG VERSION
ARG REVISION

# Set default shell
SHELL ["/bin/bash", "-c"]

# Install missing packages
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs mono-complete unzip

# Restore dependencies and tools
COPY src/ModelRepoBrowser.csproj .
RUN dotnet restore "ModelRepoBrowser.csproj"

# Set environment variables
ENV PUBLISH_DIR=/app/publish
ENV GENERATE_SOURCEMAP=false

# Create optimized production build
COPY src/ .
RUN dotnet publish "ModelRepoBrowser.csproj" \
  -c Release \
  -p:VersionPrefix=${VERSION} \
  -p:SourceRevisionId=${REVISION} \
  -o ${PUBLISH_DIR}

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ENV HOME=/app
ENV TZ=Europe/Zurich
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR ${HOME}

# Install required packages
RUN \
  DEBIAN_FRONTEND=noninteractive && \
  mkdir -p /usr/share/man/man1 /usr/share/man/man2 && \
  apt-get update && \
  apt-get install -y curl && \
  rm -rf /var/lib/apt/lists/*

EXPOSE 8080

# Set default locale
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8

COPY --from=build /app/publish $HOME

USER $APP_UID

HEALTHCHECK CMD curl --fail http://localhost:8080/ || exit 1

ENTRYPOINT ["dotnet", "ModelRepoBrowser.dll"]
