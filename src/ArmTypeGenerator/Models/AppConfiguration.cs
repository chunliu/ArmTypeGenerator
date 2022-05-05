namespace ArmTypeGenerator.Models
{
    internal class AppConfiguration
    {
        public string RootSchema { get; set; } = string.Empty;
        public string OutputDir { get; set; } = string.Empty;
        public string CopyRight { get; set; } = string.Empty;
        public IList<string> IncludedRPSchemas { get; set; } = null!;
    }
}
