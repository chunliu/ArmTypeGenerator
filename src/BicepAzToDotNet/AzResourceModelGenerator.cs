using Azure.Bicep.Types.Az;
using Azure.Bicep.Types.Concrete;
using Bicep.Core.Resources;

namespace BicepAzToDotNet
{
    public class AzResourceModelGenerator
    {
        private readonly IFileSystem _fileSystem;
        private readonly AzTypeLoader _typeLoader = new();
        private readonly AzResourceModelGeneratorSettings _settings;
        private readonly IDictionary<string, string> _generatedClasses = new Dictionary<string, string>();
        private readonly Stack<ObjectType> _objectTypeStack = new();
        private readonly IList<string> _pushedNames = new List<string>();
        public AzResourceModelGenerator(AzResourceModelGeneratorSettings settings) 
            : this(settings, new FileSystem())
        {

        }
        internal AzResourceModelGenerator(AzResourceModelGeneratorSettings settings, IFileSystem fileSystem)
        {
            _settings = settings;
            _fileSystem = fileSystem;
        }
        public static string GetLastestApiVersion(string anchorResourceType)
        {
            var typeLoader = new AzTypeLoader();
            return typeLoader.GetLatestApiVersion(anchorResourceType);
        }
        public void Generate()
        {
            //var latestApiVersion = _typeLoader.GetLatestApiVersion($"{resourceProviderName}{anchorResName}");
            //var resourceIndex = _typeLoader.FilterResourceIndex(resourceProviderName, latestApiVersion);

            var folderPath = _settings.OutputDirectory;
            _fileSystem.CreateDirectory(folderPath);

            foreach (var ri in _settings.ResourceTypes)
            {
                var tl = _typeLoader.LoadTypeIndex().Resources[$"{ri}@{_settings.ApiVersion}"];
                var resourceType = _typeLoader.LoadResourceType(tl);
                var typeRef = ResourceTypeReference.Parse(resourceType.Name);
                var className = typeRef.TypeSegments[^1];

                if (resourceType.Body.Type is not ObjectType objType)
                    throw new Exception("Resource type is not an ObjectType.");

                GenerateClass(className, objType);

                while (_objectTypeStack.Count > 0)
                {
                    var type = _objectTypeStack.Pop();
                    GenerateClass(type.Name, type);
                }
            }

            foreach(var c in _generatedClasses)
            {
                _fileSystem.WriteAllText($"{folderPath}\\{c.Key}.cs", c.Value);
            }
        }

        private string GenerateClass(string className, ObjectType resourceType)
        {
            var classGenerator = new ClassGenerator(className, resourceType, OnAdditionalTypeRequired);
            var classContent = classGenerator.Generate(_settings.Namespace, _settings.CopyrightNotice!, resourceType.Name);

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
