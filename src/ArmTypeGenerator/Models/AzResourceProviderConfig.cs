namespace ArmTypeGenerator.Models;

internal class AzResourceProviderConfig
{
    // Relative path to the root output directory
    public string ResourceProviderName { get; set; } = default!;
    public string ResourceDirectory { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public string ApiVersion { get; set; } = default!;
    public string AnchorResourceType { get; set; } = default!;
    public IList<string> ResourceTypes { get; set; } = default!;
}
