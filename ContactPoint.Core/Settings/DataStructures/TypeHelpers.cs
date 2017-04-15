using System;
using System.Linq;
using ContactPoint.Common;

namespace ContactPoint.Core.Settings.DataStructures
{
    static class TypeHelpers
    {
        public static Type GetTypeByName(string typeName, bool throwOnError = true)
        {
            return Type.GetType(typeName, false) ?? GetTypeByNameFromAllAssemblies(typeName, throwOnError);
        }

        private static Type GetTypeByNameFromAllAssemblies(string typeName, bool throwOnError = true)
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(x => x.FullName.Equals(typeName, StringComparison.InvariantCulture));
            }
            catch
            {
                Logger.LogWarn($"Failed to get type '{typeName}', throwOnError={throwOnError}");
                if (throwOnError)
                {
                    throw;
                }

                // Try to remove assembly version from type name
                var typeNameParts = typeName.Split(',').Select(x => x.Trim()).ToArray();
                if (typeNameParts.Length > 2)
                {
                    return GetTypeByName(string.Join(", ", typeNameParts.Take(2)), false);
                }
            }

            return null;
        }
    }
}