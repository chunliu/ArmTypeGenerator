namespace ArmTypeGenerator.Models;

internal class AppConfiguration
{
    public string RootOutputDirectory { get; set; } = default!;
    public string CopyrightNotice { get; set; } = default!;
    public string RootNamespace { get; set; } = default!;
    public IList<string> AzResourceConfigPath { get; set; } = default!;
}
