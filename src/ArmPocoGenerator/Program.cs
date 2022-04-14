using Microsoft.Json.Schema;
using Microsoft.Json.Schema.ToDotNet;
using Microsoft.Json.Schema.ToDotNet.Hints;

var hintText = File.ReadAllText("CodeGenHints.json");

var dmgSettings = new DataModelGeneratorSettings
{
    OutputDirectory = @"C:\Users\chunliu.FAREAST\experiment\ArmResources\ArmResources",
    RootClassName = "DeploymentTemplate",
    NamespaceName = "AzureDesignStudio.Azure",
    ForceOverwrite = true,
    HintDictionary = new HintDictionary(hintText),
    ExcludedDefinitionNames = new List<string> { "proxyResourceBase", "resourceBaseExternal", "resourceLocations", "resourceKind", "resourcesWithSymbolicName" }
};

var schema = await SchemaReader.ReadSchema(new Uri("https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json"), "deploymentTemplate.json");

var generator = new DataModelGenerator(dmgSettings);
var code = generator.Generate(schema);

Console.WriteLine(code);
Console.ReadLine();
