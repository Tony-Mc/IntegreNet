# IntegreNet
[![CI](https://github.com/Tony-Mc/IntegreNet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Tony-Mc/IntegreNet/actions/workflows/dotnet.yml)
![Nuget](https://img.shields.io/nuget/v/IntegreNet)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/IntegreNet)
![GitHub](https://img.shields.io/github/license/Tony-Mc/IntegreNet)

IntegreNet is a .NET client library for [IntegreSQL](https://github.com/allaboutapps/integresql) which manages isolated PostgreSQL databases for your integration tests.

## Installing

You can install [via NuGet](https://www.nuget.org/packages/IntegreNet):
```powershell
Install-Package IntegreNet
```

## Usage

### Docker

This can be done in multiple ways one of which is using a [`docker-compose`](https://docs.docker.com/compose/).

There's an example [`docker-compose.yml`](https://github.com/Tony-Mc/IntegreNet/blob/main/.docker/docker-compose.yml) file included in this repository. It is also used in this project's [CI workflow](https://github.com/Tony-Mc/IntegreNet/actions/workflows/dotnet.yml) (`integration` step).

In essence it is done like this:

```bash
docker-compose -f ./.docker/docker-compose.yml run -v ${PWD}:/app --workdir /app sdk dotnet test --no-build --verbosity normal ./IntegreNet.Tests.Integration.dll
```

This will launch the provided Docker configuration and run the `dotnet test` command on the provided `dll`.

### Other

Since [IntegreSQL](https://github.com/allaboutapps/integresql) is an easily runnable Docker image you might find other ways of running it your way.