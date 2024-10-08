FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY . ./

RUN dotnet tool install --global dotnet-sonarscanner

ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet build

RUN dotnet list package --outdated > outdated_packages.txt
RUN dotnet list package --vulnerable > vulnerable_packages.txt

RUN apt-get update && apt-get install -y postgresql-client

# This should be a dotnet application start
CMD ["bash", "-c", "./run_scans.sh"]

