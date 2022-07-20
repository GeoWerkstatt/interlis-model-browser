FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG VERSION
ARG REVISION

# Set default shell
SHELL ["/bin/bash", "-c"]

# Install missing packages
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs mono-complete unzip

# Restore dependencies and tools
#COPY src/ModelRepoBrowser.csproj .
#RUN dotnet restore "ModelRepoBrowser.csproj"

# Set environment variables
ENV PUBLISH_DIR=/app/publish
ENV GENERATE_SOURCEMAP=false

# Create optimized production build
#COPY src/ .
#RUN dotnet publish "ModelRepoBrowser.csproj" \
#  -c Release \
#  -p:VersionPrefix=${VERSION} \
#  -p:SourceRevisionId=${REVISION} \
#  -o ${PUBLISH_DIR}

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
ENV HOME=/app
ENV TZ=Europe/Zurich
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR ${HOME}

# Install required packages
RUN \
  DEBIAN_FRONTEND=noninteractive && \
  mkdir -p /usr/share/man/man1 /usr/share/man/man2 && \
  apt-get update && \
  apt-get install -y curl busybox && \
  rm -rf /var/lib/apt/lists/*

EXPOSE 80

# Set default locale
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8

#COPY --from=build /app/publish $HOME

#HEALTHCHECK CMD curl --fail http://localhost/ || exit 1

# For testing purposes only. The following lines
# and busybox (from apt-get install) can be removed safely
# once the C# project is ready. 
RUN \
  mkdir -p /tmp/busybox/www && \
  echo "Hello from ilimodels.ch" >> /tmp/busybox/www/index.html

ENTRYPOINT busybox httpd -h /tmp/busybox/www -p 3080 && tail -f /dev/null

#ENTRYPOINT ["dotnet", "ModelRepoBrowser.dll"]
