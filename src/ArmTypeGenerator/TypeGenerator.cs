using ArmTypeGenerator.Models;
using Microsoft.Json.Schema;
using Microsoft.Json.Schema.ToDotNet;
using Microsoft.Json.Schema.ToDotNet.Hints;
using Spectre.Console;
using System.Text.Json;
using System.Text.RegularExpressions;
using System;

namespace ArmTypeGenerator
{
    internal static class TypeGenerator
    {
        private static readonly Regex s_childResourceRegex = new(@"(?<className>.+)_childResource$");
        internal static async Task GenerateTypesForDeploymentTemplate()
        {
            try
            {
                var config = File.ReadAllText("Configs\\DeploymentTemplateConfig.json");
                var deploymentTemplateSettings = Helper.GetDMGSettings(config);
                if (deploymentTemplateSettings == null)
                    throw new Exception("Failed to get the DataModelGeneratorSettings!");
                // Get output dir from the app configurations.
                deploymentTemplateSettings.OutputDirectory = Helper.AppConfiguration.OutputDir;

                await AnsiConsole.Status()
                    .StartAsync("Generating types...", async ctx =>
                    {
                        var dtSchema = await SchemaReader.ReadSchema(
                            new Uri(Helper.AppConfiguration.RootSchema),
                            "deploymentTemplate.json");
                        AnsiConsole.MarkupLine("Read the schema of DeploymentTemplate.");

                        // Update the status and spinner
                        ctx.Status("Generating types from the schema...");
                        ctx.Spinner(Spinner.Known.Star);
                        ctx.SpinnerStyle(Style.Parse("green"));

                        var dtGenerator = new DataModelGenerator(deploymentTemplateSettings);
                        _ = dtGenerator.Generate(dtSchema);

                        AnsiConsole.MarkupLine("[green]Types for DeploymentTemplate are generated.[/]");
                        AnsiConsole.WriteLine();
                    });
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }

            await Helper.MainFlow();
        }

        internal static async Task GenerateTypesForResourceProviders()
        {
            try
            {
                var rpList = ReadRPSchemaList();
                foreach (var rp in rpList)
                {
                    var rpConfig = File.ReadAllText($"Configs\\{rp.Name}.Config.json");
                    var rpSettings = Helper.GetDMGSettings(rpConfig);
                    if (rpSettings == null)
                        throw new Exception("Failed to get the DataModelGeneratorSettings!");
                    // Get output dir from the app configurations.
                    rpSettings.OutputDirectory = Helper.AppConfiguration.OutputDir;

                    var confirm = AnsiConsole.Confirm($"Generate types for {rp.Name}?");
                    if (!confirm)
                        continue;

                    await AnsiConsole.Status()
                        .StartAsync($"Generating types for {rp.Name}...", async ctx =>
                        {
                            var schemaUri = new Uri(rp.SchemaUrl);
                            rpSettings.NamespaceDir = schemaUri.Segments[^1].Split('.')[1];

                            var rdSchema = await SchemaReader.ReadSchema(
                                schemaUri,
                                $"{rp.Name}.json");
                            AnsiConsole.MarkupLine($"Load the schema of {rp.Name}.");

                            // Update the status and spinner
                            ctx.Status("Generating types from the schema...");
                            ctx.Spinner(Spinner.Known.Star);
                            ctx.SpinnerStyle(Style.Parse("green"));

                            PrepareDMGSettingsForResourceDefinitions(rdSchema, ref rpSettings);

                            var rdGenerator = new DataModelGenerator(rpSettings);
                            rdGenerator.GenerateClassesForResourceDefinitions(rdSchema);
                            AnsiConsole.MarkupLine($"[green]Types for {rp.Name} are generated.[/]");
                        });
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }

            await Helper.MainFlow();
        }
        internal static string ToCamelCase(this string s)
        {
            return string.Concat(s[0].ToString().ToLowerInvariant(), s.AsSpan(1));
        }

        private static void PrepareDMGSettingsForResourceDefinitions(JsonSchema rdSchema, ref DataModelGeneratorSettings dmgSettings)
        {
            var excludedDefinitions = new List<string>();
            foreach (var definition in rdSchema.Definitions)
            {
                Match match = s_childResourceRegex.Match(definition.Key);
                if (match.Success)
                {
                    excludedDefinitions.Add(definition.Key);
                    var className = match.Groups["className"].Captures[0].Value;
                    ClassNameHint classNameHint = new(className.ToCamelCase());
                    dmgSettings.HintDictionary.Add(definition.Key, new List<CodeGenHint> { classNameHint }.ToArray());
                }
            }
            if (dmgSettings.ExcludedDefinitionNames?.Count > 0)
            {
                dmgSettings.ExcludedDefinitionNames.ToList().AddRange(excludedDefinitions);
            }
            else
            {
                 dmgSettings.ExcludedDefinitionNames = excludedDefinitions;
            }
        }

        internal static async Task GetRPSchemas()
        {
            try
            {
                var table = new Table().Centered();
                var outputList = new List<ResourceProvider>();

                await AnsiConsole.Live(table)
                    .StartAsync(async ctx =>
                    {
                        table.AddColumn("Name");
                        table.AddColumn("Version");
                        table.AddColumn("Url");
                        ctx.Refresh();

                        var dtSchema = await SchemaReader.ReadSchema(
                            new Uri(Helper.AppConfiguration.RootSchema),
                            "deploymentTemplate.json");

                        var resourceSchema = dtSchema.Definitions.FirstOrDefault(s => s.Key == "resource").Value;
                        var rpList = PopulateRPList(resourceSchema.OneOf[0].AllOf[1].OneOf.Select(r => r.Reference).ToList());

                        foreach (var rp in rpList)
                        {
                            table.AddRow(rp.Name!, rp.Version!, rp.SchemaUrl!);
                        }
                        ctx.Refresh();

                        var autogenSchema = await SchemaReader.ReadSchema(
                            resourceSchema.OneOf[3].Reference.Uri,
                            "autogeneratedResources.json");

                        var agList = PopulateRPList(autogenSchema.AllOf[1].OneOf.Select(r => r.Reference).ToList());
                        foreach (var rp in agList)
                        {
                            table.AddRow(rp.Name!, rp.Version!, rp.SchemaUrl!);
                        }
                        ctx.Refresh();

                        // Merge 2 lists
                        outputList.AddRange(rpList);
                        outputList.AddRange(agList);
                    });

                SaveRPSchemaList(outputList);

                AnsiConsole.MarkupLine("[green]The schema list of RPs has been saved.[/]");
                AnsiConsole.WriteLine();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }

            await Helper.MainFlow();
        }

        private static IList<ResourceProvider> PopulateRPList(IList<UriOrFragment> referenceList)
        {
            var list = new List<ResourceProvider>();

            foreach (var reference in referenceList)
            {
                var rp = new ResourceProvider
                {
                    Name = reference.Uri.Segments[3].Replace(".json", ""),
                    Version = reference.Uri.Segments[2].Replace("/", ""),
                    SchemaUrl = reference.Uri.OriginalString.Replace(reference.Uri.Fragment, ""),
                };

                if (Helper.AppConfiguration.IncludedRPSchemas.Contains(rp.Name))
                    list.Add(rp);
            }

            var result = new List<ResourceProvider>();

            foreach (var group in list.OrderByDescending(s => s.VersionDate).GroupBy(r => r.Name).ToList())
            {
                result.Add(group.First());
            }

            return result;
        }

        private static void SaveRPSchemaList(IList<ResourceProvider> list)
        {
            var listStr = JsonSerializer.Serialize(list,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    });

            File.WriteAllText("Configs\\RPSchemasList.json", listStr);
        }

        private static IList<ResourceProvider> ReadRPSchemaList()
        {
            var listStr = File.ReadAllText("Configs\\RPSchemasList.json");

            return JsonSerializer.Deserialize<IList<ResourceProvider>>(listStr,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    })!;
        }
    }
}
