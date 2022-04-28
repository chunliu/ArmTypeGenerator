using ArmPocoGenerator.Models;
using Microsoft.Json.Schema.ToDotNet;
using Spectre.Console;
using System.Text.Json;

namespace ArmPocoGenerator
{
    internal static class Helper
    {
        public static AppConfiguration AppConfiguration { get; set; } = null!;
        public static void LoadAppConfiguration()
        {
            var config = File.ReadAllText("Configs\\AppConfiguration.json");
            AppConfiguration = JsonSerializer.Deserialize<AppConfiguration>(
                config,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                })!;
        }
        internal static DataModelGeneratorSettings? GetDMGSettings(string config)
        {
            return JsonSerializer.Deserialize<DataModelGeneratorSettings>(
                config,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new HintDictionaryConverter() }
                }
            );
        }

        public static async Task MainFlow()
        {
            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("Choose an option below:")
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(new[] {
                        0, 1, 2, 3
                    })
                    .UseConverter(opt => opt switch
                    {
                        0 => "Generate types for Deployment Template",
                        1 => "Get latest json schemas for RPs",
                        2 => "Gererate types for RPs",
                        3 => "Exit",
                        _ => ""
                    }));

            switch (selectedOption)
            {
                case 0:
                    await ArmTypeGenerator.GenerateTypesForDeploymentTemplate();
                    break;
                case 1:
                    await ArmTypeGenerator.GetRPSchemas();
                    break;
                case 2:
                    await ArmTypeGenerator.GenerateTypesForResourceProviders();
                    break;
                case 3:
                default:
                    break;
            }
        }
    }
}
