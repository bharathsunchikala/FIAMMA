---
title: ".NET Maintenance"
parent: Explore
---

When we need to update to a new version of .NET, the following .NET references need to be updated:

## Root

- [Directory.Packages.props](https://github.com/NimblePros/Fiamma/blob/main/Directory.Packages.props)
- [global.json](https://github.com/NimblePros/Fiamma/blob/main/global.json)
- [README](https://github.com/NimblePros/Fiamma/blob/main/README.md) - see the VERSIONS header.

## Fiamma.AppHost

- [Fiamma.AppHost.csproj](https://github.com/NimblePros/Fiamma/blob/main/src/Fiamma.AppHost/Fiamma.AppHost.csproj)

## Fiamma.AspireServiceDefaults

- [Fiamma.AspireServiceDefaults.csproj](https://github.com/NimblePros/Fiamma/blob/main/src/Fiamma.AspireServiceDefaults/Fiamma.AspireServiceDefaults.csproj)

## PublicApi

- [PublicApi Dockerfile](https://github.com/NimblePros/Fiamma/blob/main/src/PublicApi/Dockerfile)

## Web

- [Web Dockerfile](https://github.com/NimblePros/Fiamma/blob/main/src/Web/Dockerfile)
- [dotnet tools config in Web](https://github.com/NimblePros/Fiamma/blob/main/src/Web/.config/dotnet-tools.json)


- [.vscode/launch.json](https://github.com/NimblePros/Fiamma/blob/main/.vscode/launch.json) - check `configurations.program` folder path

- [infra/main.bicep](https://github.com/NimblePros/Fiamma/blob/main/infra/main.bicep) - `runtimeVersion`

