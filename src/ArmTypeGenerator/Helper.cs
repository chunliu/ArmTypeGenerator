using ArmTypeGenerator.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArmTypeGenerator
{
    internal static class Helper
    {
        internal static AppConfiguration AppConfiguration { get; set; } = default!;
        internal static List<AzResourceProviderConfig> AzResourceProviderConfigs { get; set; } = new();
        internal static void LoadAppConfiguration()
        {
            var config = File.ReadAllText("Configs\\AppConfiguration.json");
            AppConfiguration = JsonSerializer.Deserialize<AppConfiguration>(
                config,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                })!;

            foreach(var configPath in AppConfiguration.AzResourceProviderConfigs)
            {
                var jsonText = File.ReadAllText($"Configs\\{configPath}");
                var resConfig = JsonSerializer.Deserialize<AzResourceProviderConfig>(
                    jsonText,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    })!;

                AzResourceProviderConfigs.Add(resConfig);
            }
        }

        internal static async Task MainFlow()
        {
            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("Choose an option below:")
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(new[] {
                        0, 1, 2
                    })
                    .UseConverter(opt => opt switch
                    {
                        0 => "1. Check api version",
                        1 => "2. Generate types for RPs",
                        2 => "3. Exit",
                        _ => ""
                    }));

            switch(selectedOption)
            {
                case 0:
                    await TypeGenerator.GetLatestApiVersions(); 
                    break;
                case 1:
                    await TypeGenerator.GenerateTypesForResourceProviders();
                    break;
                default:
                    break;
            }
        }
    }
}
