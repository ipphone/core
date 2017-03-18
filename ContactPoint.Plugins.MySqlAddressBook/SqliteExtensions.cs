using System;
using System.Data;

namespace ContactPoint.Plugins.MySqlAddressBook
{
    internal static class SqliteExtensions
    {
        public static string GetStringSafe(this IDataReader reader, int number)
        {
            if (reader.IsDBNull(number)) return String.Empty;

            return reader.GetString(number);
        }
    }
}
