using ArmTypeGenerator;
using BicepAzToDotNet;
using Spectre.Console;
using System.Reflection;

Helper.LoadAppConfiguration();

AnsiConsole.Write(
    new FigletText("ARM Type Generator")
    .Color(Color.Cyan1));
AnsiConsole.MarkupLine($"ArmTypeGenerator, " +
    $"v{Assembly.GetExecutingAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion}");
AnsiConsole.WriteLine();

//var settings = new AzResourceModelGeneratorSettings
//{
//    OutputDirectory = @"D:\Code\github\AzureDesignStudio\src\AzureDesignStudio.AzureResources\Network",
//    Namespace = "AzureDesignStudio.AzureResources.Network",
//    CopyrightNotice = "// Licensed under the MIT License. See LICENSE in the project root for license information.\n\r",
//    ApiVersion = "2022-05-01",
//    ResourceTypes = new List<string>()
//    {
//        "Microsoft.Network/virtualNetworks",
//        "Microsoft.Network/virtualNetworks/subnets"
//    }
//};

//var generator = new AzResourceModelGenerator(settings);
//generator.Generate();

//Console.WriteLine("Done!");
//Console.ReadLine();

await Helper.MainFlow();