namespace BicepAzToDotNet
{
    internal static class StringExtensions
    {
        internal static string ToPascalCase(this string s)
        {
            return string.Concat(s[0].ToString().ToUpperInvariant(), s.AsSpan(1));
        }
        internal static string ToCamelCase(this string s)
        {
            return string.Concat(s[0].ToString().ToLowerInvariant(), s.AsSpan(1));
        }
    }
}
