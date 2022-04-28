# ArmTypeGenerator

[![CI/CD](https://github.com/chunliu/ArmTypeGenerator/actions/workflows/release.yml/badge.svg)](https://github.com/chunliu/ArmTypeGenerator/actions/workflows/release.yml)

A CLI to generator C# types for Azure ARM resources from the corresponding json schemas of Azure ARM.

![ArmTypeGenerator screenshot](ArmTypeGenerator.jpg)

## How to use

1. Download the latest version from [releases](https://github.com/chunliu/ArmTypeGenerator/releases).
1. Unzip it to a folder. Optionally, add the folder path to the `PATH` of your environment.
1. Update the `outputDir` setting in the `Configs/AppConfiguration.json` to the absolute path of the folder where you want the generated type files to be in.
1. Run `ArmTypeGenerator.exe` from command line.
1. Follow the instructions.

## Support resource providers

The following resource providers are supported:

- Microsoft.Network
- Microsoft.Sql

More resource providers will be supported over time.
