# ArmTypeGenerator

[![CI/CD](https://github.com/chunliu/ArmTypeGenerator/actions/workflows/release.yml/badge.svg)](https://github.com/chunliu/ArmTypeGenerator/actions/workflows/release.yml)

A console application to generator C# types for Azure ARM resources from the corresponding json schemas of Azure ARM. A set of generated libraries can be found in this repo: [AzureDesignStudio.AzureResources](https://github.com/chunliu/AzureDesignStudio.AzureResources).

![ArmTypeGenerator screenshot](ArmTypeGenerator.jpg)

## How to use

1. Download the latest version from [releases](https://github.com/chunliu/ArmTypeGenerator/releases).
1. Unzip it to a folder. Optionally, add the folder path to the `PATH` of your environment.
1. Update the `outputDir` setting in the `Configs/AppConfiguration.json` to the absolute path of the folder where you want the generated type files to be in.
1. Run `ArmTypeGenerator.exe` from the terminal.
1. Follow the instructions.

## Acknowledgement

The core functions of ArmTypeGenerator is built on top of a customized version of `Microsoft.Json.Schema` and `Microsoft.Json.Schema.ToDotNet` which are parts of [Microsoft/jschema](https://github.com/microsoft/jschema). Microsoft/jschema is the most complete and useful library for C# to deal with Json schemas that I can find so far. Thanks my colleague [@cbellee](https://github.com/cbellee) for recommending it to me.

The console is built on top of the wonderful library [Spectre.Console](https://spectreconsole.net/).
