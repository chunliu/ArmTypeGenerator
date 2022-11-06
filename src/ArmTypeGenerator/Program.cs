using BicepAzToDotNet;
using Spectre.Console;
using System.Reflection;

AnsiConsole.Write(
    new FigletText("ARM Type Generator")
    .Color(Color.Cyan1));
AnsiConsole.MarkupLine($"ArmTypeGenerator, " +
    $"v{Assembly.GetExecutingAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion}");
AnsiConsole.WriteLine();

var generator = new AzResourceModelGenerator();
generator.Generate(@"Microsoft.Network/", @"virtualNetworks");

Console.ReadLine();