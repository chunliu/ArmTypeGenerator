using Azure.Bicep.Types.Az;
using Azure.Bicep.Types.Concrete;
using Bicep.Core.Resources;

namespace BicepAzToDotNet
{
    public class AzResourceModelGenerator
    {
        private readonly IFileSystem _fileSystem;
        private readonly AzTypeLoader _typeLoader = new();
        private readonly IDictionary<string, string> _generatedClasses = new Dictionary<string, string>();
        private readonly Stack<ObjectType> _objectTypeStack = new();
        private readonly IList<string> _pushedNames = new List<string>();
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

            var folderPath = @"D:\Code\github\AzureDesignStudio.AzureResources\src\Network";

            _fileSystem.CreateDirectory(folderPath);

            //foreach(var ri in resourceIndex)
            //{
            var ri = _typeLoader.LoadTypeIndex().Resources[$"{resourceProviderName}{anchorResName}@{latestApiVersion}"];
                var resourceType = _typeLoader.LoadResourceType(ri);
                var typeRef = ResourceTypeReference.Parse(resourceType.Name);
                var className = typeRef.TypeSegments[^1];

            if (resourceType.Body.Type is not ObjectType objType)
                throw new Exception("Resource type is not an ObjectType.");


                var csharpText = GenerateClass(className, objType);
            //}

            while(_objectTypeStack.Count > 0)
            {
                var type = _objectTypeStack.Pop();
                GenerateClass(type.Name, type);
            }

            foreach(var c in _generatedClasses)
            {
                _fileSystem.WriteAllText($"{folderPath}\\{c.Key}.cs", c.Value);
            }

            return csharpText;
        }

        private string GenerateClass(string className, ObjectType resourceType)
        {
            var classGenerator = new ClassGenerator(className, resourceType, OnAdditionalTypeRequired);
            var classContent = classGenerator.Generate($"GenerateTest", "licensed", resourceType.Name);

            _generatedClasses[className.ToPascalCase()] = classContent;

            return classContent;
        }
        private void OnAdditionalTypeRequired(ObjectType resourceType)
        {
            if (!_pushedNames.Contains(resourceType.Name))
            {
                _objectTypeStack.Push(resourceType);
                _pushedNames.Add(resourceType.Name);
            }
        }
    }
}
