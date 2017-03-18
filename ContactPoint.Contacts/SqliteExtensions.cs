using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ContactPoint.Contacts
{
    internal static class SqliteExtensions
    {
        public static string GetStringSafe(this SQLiteDataReader reader, int number)
        {
            if (reader.IsDBNull(number)) return String.Empty;

            return reader.GetString(number);
        }
    }
}
