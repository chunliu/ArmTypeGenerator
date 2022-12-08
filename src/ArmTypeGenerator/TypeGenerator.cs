using ArmTypeGenerator.Models;
using BicepAzToDotNet;
using Spectre.Console;

namespace ArmTypeGenerator;

internal static class TypeGenerator
{
    internal static async Task GetLatestApiVersions()
    {
        try
        {
            var table = new Table().Centered();

            await AnsiConsole.Live(table).StartAsync(ctx =>
            {
                table.AddColumn("Azure Resource");
                table.AddColumn("Current Api Version");
                table.AddColumn("Latest Api Version");

                foreach(var resConfig in Helper.AzResourceProviderConfigs)
                {
                    table.AddRow(resConfig.AnchorResourceType,
                        resConfig.ApiVersion,
                        AzResourceModelGenerator.GetLastestApiVersion(resConfig.AnchorResourceType));
                }

                ctx.Refresh();

                return Task.FromResult(Task.CompletedTask);
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
            foreach(var rpConfig in Helper.AzResourceProviderConfigs)
            {
                var confirm = AnsiConsole.Confirm($"Generate types for {rpConfig.ResourceProviderName}?");
                if (!confirm)
                {
                    continue;
                }

                await AnsiConsole.Status().StartAsync($"Generating types for {rpConfig.ResourceProviderName}",
                    ctx =>
                    {
                        var settings = new AzResourceModelGeneratorSettings()
                        {
                            OutputDirectory = $"{Helper.AppConfiguration.RootOutputDirectory}\\{rpConfig.ResourceDirectory}",
                            Namespace = $"{Helper.AppConfiguration.RootNamespace}.{rpConfig.Namespace}",
                            CopyrightNotice = Helper.AppConfiguration.CopyrightNotice,
                            ApiVersion = rpConfig.ApiVersion,
                            ResourceTypes = rpConfig.ResourceTypes,
                        };
                        var generator = new AzResourceModelGenerator(settings);
                        generator.Generate();

                        AnsiConsole.MarkupLine($"[green]Types for {rpConfig.ResourceProviderName} are generated.[/]");

                        return Task.FromResult(Task.CompletedTask);
                    });
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        await Helper.MainFlow();
    }
}
