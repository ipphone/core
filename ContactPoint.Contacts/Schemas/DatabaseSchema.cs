using System;
using System.Data.SQLite;
using ContactPoint.Common;

namespace ContactPoint.Contacts.Schemas
{
    internal abstract class DatabaseSchema
    {
        public static DatabaseSchema LatestSchema = new SchemaV2();
        public abstract int Version { get; }

        public static void Upgrade(SQLiteConnection connection)
        {
            Logger.LogNotice("Trying to upgrade DB schema");

            int version = 0;
            try
            {
                Logger.LogNotice("Trying to get DB schema version");
                version = LatestSchema.GetVersion(connection);
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Exception while getting DB schema version");
            }

            Logger.LogNotice($"Current DB schema version is '{version}'");
            DatabaseSchema schema;
            switch (version)
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

            if (schema.Version < LatestSchema.Version)
            {
                Logger.LogNotice($"Trying to upgrade DB schema of version '{version}' to '{LatestSchema.Version}'");
                LatestSchema.Upgrade(connection, schema);
            }
            else
            {
                Logger.LogNotice($"DB schema of version '{version}' is up to date - nothing to upgrade");
            }
        }

        public abstract void Upgrade(SQLiteConnection connection, DatabaseSchema currentSchema);
        protected abstract int GetVersion(SQLiteConnection connection);
    }
}
