namespace BicepAzToDotNet
{
    public class AzResourceModelGeneratorSettings
    {
        public string OutputDirectory { get; set; } = default!;
        public string Namespace { get; set; } = default!;
        public string? CopyrightNotice { get; set; }
        public string? ApiVersion { get; set; }
        public IList<string> ResourceTypes { get; set; } = default!;
    }
}
