using System.Data.SQLite;

namespace ContactPoint.Contacts.Schemas
{
    internal class SchemaV1 : InitialSchema
    {
        public override int Version => 1;

        public override void Upgrade(SQLiteConnection connection, DatabaseSchema currentSchema)
        {
            base.Upgrade(connection, currentSchema);

            if (currentSchema.Version < Version)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS [info] (
[name] nvarchar(50) NOT NULL,
[value_int] INTEGER NULL,
[value_string] NVARCHAR(255) NULL,
[id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
);

delete from info where name = 'db_version';
insert into info(name, value_int) values ('db_version', 1);
";

                    command.ExecuteNonQuery();
                }
            }
        }

        protected override int GetVersion(SQLiteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select value_int from [info] where name = 'db_version'";

                try
                {
                    return (int)(long)command.ExecuteScalar();
                }
                catch
                {
                    return base.GetVersion(connection);
                }
            }
        }
    }
}
