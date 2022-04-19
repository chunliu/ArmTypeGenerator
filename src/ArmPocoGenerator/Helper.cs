using Microsoft.Json.Schema.ToDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArmPocoGenerator
{
    internal static class Helper
    {
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
    }
}
