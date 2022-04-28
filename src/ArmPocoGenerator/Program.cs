using ArmTypeGenerator;
using Spectre.Console;
using System.Reflection;

Helper.LoadAppConfiguration();

AnsiConsole.Write(
    new FigletText("ARM Type Generator")
    .Color(Color.Cyan1));
AnsiConsole.MarkupLine($"ArmTypeGenerator, " +
    $"v{Assembly.GetExecutingAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion}");
AnsiConsole.WriteLine();

await Helper.MainFlow();
