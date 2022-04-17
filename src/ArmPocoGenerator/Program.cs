using ArmPocoGenerator;
using Microsoft.Json.Schema;
using Microsoft.Json.Schema.ToDotNet;
using System.Text.Json;

var config = File.ReadAllText("DeploymentTemplateConfig.json");
var deploymentTemplateSettings = JsonSerializer.Deserialize<DataModelGeneratorSettings>(
    config, 
    new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new HintDictionaryConverter() }
    }
);

var schema = await SchemaReader.ReadSchema(
    new Uri("https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json"), 
    "deploymentTemplate.json");

var generator = new DataModelGenerator(deploymentTemplateSettings);
var code = generator.Generate(schema);

Console.WriteLine(code);
Console.ReadLine();
