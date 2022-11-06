using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BicepAzToDotNet
{
    internal class UsingComparer : IComparer<string>
    {
        internal static readonly UsingComparer Instance = new UsingComparer();

        public int Compare(string? first, string? second)
        {
            bool firstIsSystemUsing = IsSystemUsing(first!);
            bool secondIsSystemUsing = IsSystemUsing(second!);

            if (firstIsSystemUsing && !secondIsSystemUsing)
            {
                return -1;
            }

            if (!firstIsSystemUsing && secondIsSystemUsing)
            {
                return 1;
            }

            return first!.CompareTo(second);
        }

        private const string SystemNamespaceName = "System";
        private const string SystemUsingPrefix = SystemNamespaceName + ".";

        private static bool IsSystemUsing(string namespaceName)
        {
            return namespaceName.Equals(SystemNamespaceName)
                || namespaceName.StartsWith(SystemUsingPrefix);
        }
    }

}
