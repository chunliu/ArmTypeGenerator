using ArmTypeGenerator.Models;
using Microsoft.Json.Schema.ToDotNet;
using Spectre.Console;
using System.Text.Json;

namespace ArmTypeGenerator
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
                        0 => "1. Generate types for Deployment Template",
                        1 => "2. Get latest json schemas for RPs",
                        2 => "3. Gererate types for RPs",
                        3 => "4. Exit",
                        _ => ""
                    }));

            switch (selectedOption)
            {
                case 0:
                    await TypeGenerator.GenerateTypesForDeploymentTemplate();
                    break;
                case 1:
                    await TypeGenerator.GetRPSchemas();
                    break;
                case 2:
                    await TypeGenerator.GenerateTypesForResourceProviders();
                    break;
                case 3:
                default:
                    break;
            }
        }
    }
}
