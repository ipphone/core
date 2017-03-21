using System.Data.SQLite;

namespace ContactPoint.Contacts.Schemas
{
    internal abstract class DatabaseSchema
    {
        public static DatabaseSchema LatestSchema = new SchemaV2();
        public abstract int Version { get; }

        public static void Upgrade(SQLiteConnection connection)
        {
            DatabaseSchema schema;
            switch (LatestSchema.GetVersion(connection))
            {
                case 1:
                    schema = new SchemaV1();
                    break;
                case 2:
                    schema = new SchemaV2();
                    break;
                default:
                    schema = new InitialSchema();
                    break;
            }

            LatestSchema.Upgrade(connection, schema);
        }

        public abstract void Upgrade(SQLiteConnection connection, DatabaseSchema currentSchema);
        protected abstract int GetVersion(SQLiteConnection connection);
    }
}
