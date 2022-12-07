namespace ArmTypeGenerator.Models;

internal class AzResourceConfiguration
{
    // Relative path to the root output directory
    public string ResourceDirectory { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public string ApiVersion { get; set; } = default!;
    public string AnchorResourceType { get; set; } = default!;
    public IList<string> ResourceTypes { get; set; } = default!;
}
