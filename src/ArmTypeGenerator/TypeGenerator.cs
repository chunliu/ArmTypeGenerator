using BicepAzToDotNet;
using Spectre.Console;

namespace ArmTypeGenerator
{
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
                    table.AddColumn("Api Version");

                    foreach(var resConfig in Helper.AzResourceConfigs)
                    {
                        table.AddRow(resConfig.AnchorResourceType,
                            AzResourceModelGenerator.GetLastestApiVersion(resConfig.AnchorResourceType));
                    }

                    ctx.Refresh();

                    return Task.FromResult(Task.CompletedTask);
                });
            }
            catch 
            { 
            }
        }
    }
}
