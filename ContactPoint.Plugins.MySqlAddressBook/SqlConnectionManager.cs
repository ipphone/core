using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common;
using MySql.Data.MySqlClient;

namespace ContactPoint.Plugins.MySqlAddressBook
{
    internal class SqlConnectionManager
    {
        private string _connectionString = String.Empty;

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public SqlConnectionManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection OpenConnection()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                connection.Open();

                return connection;
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Couldn't open connection to MySQL server.");
                throw;
            }
        }
    }
}
