using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;

namespace BicepAzToDotNet
{
    internal static class AzTypeLoaderExtensions
    {
        internal static IDictionary<string, TypeLocation> FilterResourceIndex(this AzTypeLoader typeLoader, 
            string resourceProviderName, string apiVersion)
        {
            return typeLoader.LoadTypeIndex().Resources
                .Where(r => (r.Key.Contains(resourceProviderName) && r.Key.Contains(apiVersion)))
                .ToDictionary(r => r.Key, r => r.Value);
        }

        internal static string GetLatestApiVersion(this AzTypeLoader typeLoader, string resourceProviderName)
        {
            var apiVersions = typeLoader.LoadTypeIndex().Resources
                .Where(r => r.Key.Contains(resourceProviderName))
                .Select(r => r.Key.Split('@')[1])
                .OrderByDescending(s => s)
                .GroupBy(s => s)
                .ToList();

            return apiVersions.First().Key;
        }
    }
}
