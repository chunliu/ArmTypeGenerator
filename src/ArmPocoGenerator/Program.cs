using ArmPocoGenerator;
using Spectre.Console;

Helper.LoadAppConfiguration();

AnsiConsole.Write(
    new FigletText("ARM Type Generator")
    .Color(Color.Cyan1));

await Helper.MainFlow();
