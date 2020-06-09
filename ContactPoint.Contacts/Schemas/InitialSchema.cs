using System.Data.SQLite;
using ContactPoint.Common;

namespace ContactPoint.Contacts.Schemas
{
    internal class InitialSchema : DatabaseSchema
    {
        public override int Version => 0;

        public static void Create(SQLiteConnection connection)
        {
            Logger.LogNotice("Creating initial DB schema");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"CREATE TABLE [addressbooks] (
[name] NVARCHAR(255)  NOT NULL,
[lastupdate] TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
[key] VARCHAR(36)  UNIQUE NOT NULL,
[id] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL
);

CREATE TABLE [contact_infos] (
[id] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[addressbook_id] INTEGER  NOT NULL,
[key] NVARCHAR(50)  NULL,
[first_name] NVARCHAR(255)  NULL,
[last_name] NVARCHAR(255)  NULL,
[middle_name] NVARCHAR(255)  NULL,
[company] NVARCHAR(255)  NULL,
[job_title] NVARCHAR(255)  NULL,
[note] TEXT  NULL,
[version_tag] NVARCHAR(255) NULL
);

CREATE TABLE [contact_info_phones] (
[id] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[contact_info_id] INTEGER NOT NULL,
[key] NVARCHAR(50)  NULL,
[number] NVARCHAR(50) NOT NULL,
[comment] NVARCHAR(255) NULL,
[version_tag] NVARCHAR(255) NULL
);

CREATE TABLE [contact_info_emails] (
[id] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[contact_info_id] INTEGER NOT NULL,
[key] NVARCHAR(50)  NULL,
[email] NVARCHAR(50) NOT NULL,
[comment] NVARCHAR(255) NULL,
[version_tag] NVARCHAR(255) NULL
);

CREATE TABLE [tags] (
[id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
[addressbook_id] INTEGER  NOT NULL,
[key] NVARCHAR(50)  NULL,
[version_tag] NVARCHAR(255) NULL,
[name] NVARCHAR(50) NOT NULL,
[color] NVARCHAR(16) DEFAULT '' NOT NULL
);

CREATE TABLE [tags_links] (
[tag_id] INTEGER  NOT NULL,
[contact_info_id] INTEGER  NOT NULL
);

CREATE TABLE [contacts] (
[id] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[first_name] NVARCHAR(255)  NULL,
[last_name] NVARCHAR(255)  NULL,
[company] NVARCHAR(255)  NULL,
[middle_name] NVARCHAR(255)  NULL
);

CREATE TABLE [contacts_links] (
[contact_info_id] INTEGER  NOT NULL,
[contact_id] INTEGER  NOT NULL
);

CREATE INDEX [IDX_CONTACT_INFOS_ADDRESSBOOK_ID] ON [contact_infos](
[addressbook_id]  ASC
);

CREATE UNIQUE INDEX [IDX_CONTACTS_LINKS_CONTACT_INFO_ID] ON [contacts_links](
[contact_info_id]  ASC,
[contact_id]  ASC
);

";

                command.ExecuteNonQuery();
            }

        }

        public override void Upgrade(SQLiteConnection connection, DatabaseSchema oldSchema)
        { }

        protected override int GetVersion(SQLiteConnection connection)
        {
            return 0;
        }
    }
}
