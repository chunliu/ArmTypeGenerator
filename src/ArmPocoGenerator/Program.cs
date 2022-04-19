using ArmPocoGenerator;
using Microsoft.Json.Schema;
using Microsoft.Json.Schema.ToDotNet;

var config = File.ReadAllText("DeploymentTemplateConfig.json");
var deploymentTemplateSettings = Helper.GetDMGSettings(config);

var dtSchema = await SchemaReader.ReadSchema(
    new Uri("https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json"), 
    "deploymentTemplate.json");

Console.WriteLine("Generating classes for deployment template...");

var dtGenerator = new DataModelGenerator(deploymentTemplateSettings);
_ = dtGenerator.Generate(dtSchema);

Console.WriteLine("Classes for deployment template are generated.");

Console.WriteLine("Generating classes for resource definitions...");

var rdConfig = File.ReadAllText("ResourceDefinitionsConfig.json");
var rdSettings = Helper.GetDMGSettings(rdConfig)!;
var schemaUri = new Uri("https://schema.management.azure.com/schemas/2020-11-01/Microsoft.Network.json");

rdSettings.NamespaceDir = schemaUri.Segments[^1].Split('.')[1];

var rdSchema = await SchemaReader.ReadSchema(
    schemaUri,
    "Microsoft.Network.json"
    );
var rdGenerator = new DataModelGenerator(rdSettings);
rdGenerator.GenerateClassesForResourceDefinitions(rdSchema);

Console.WriteLine("Classes for resource definitions are generated.");

Console.ReadLine();
