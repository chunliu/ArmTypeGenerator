using Azure.Bicep.Types.Az;
using Azure.Bicep.Types.Concrete;
using Bicep.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace BicepAzToDotNet
{
    public class AzResourceModelGenerator
    {
        private readonly IFileSystem _fileSystem;
        private readonly AzTypeLoader _typeLoader = new();
        public AzResourceModelGenerator() : this(new FileSystem())
        {

        }
        internal AzResourceModelGenerator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        public string Generate(string resourceProviderName, string anchorResName)
        {
            var latestApiVersion = _typeLoader.GetLatestApiVersion($"{resourceProviderName}{anchorResName}");
            var resourceIndex = _typeLoader.FilterResourceIndex(resourceProviderName, latestApiVersion);

            //foreach(var ri in resourceIndex)
            //{
            var ri = _typeLoader.LoadTypeIndex().Resources[$"{resourceProviderName}{anchorResName}@{latestApiVersion}"];
                var resourceType = _typeLoader.LoadResourceType(ri);
                var typeRef = ResourceTypeReference.Parse(resourceType.Name);
                var className = typeRef.TypeSegments[^1];

                var csharpText = GenerateClass(className, resourceType.Body.Type);
            //}

            return string.Empty;
        }
        private string GenerateClass(string className, TypeBase resourceType)
        {
            _fileSystem.CreateDirectory($"D:\\Code\\experiments\\GeneratedTypes");

            var classGenerator = new ClassGenerator(className, resourceType);
            var classContent = classGenerator.Generate($"GenerateTest", "licensed", "This is description");

            return string.Empty;
        }
    }
}
