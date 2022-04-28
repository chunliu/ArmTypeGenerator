using System.Text.Json.Serialization;

namespace ArmTypeGenerator.Models
{
    internal class ResourceProvider
    {
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime VersionDate => DateTime.Parse(Version
                        .Replace("-preview", "")
                        .Replace("-privatepreview", ""));
        public string Version { get; set; } = string.Empty;
        public string SchemaUrl { get; set; } = string.Empty;
    }
}
