using Dapper;
using Microsoft.Data.Sqlite;
using AspNetCoreDemo.Database.Contracts;
using System.Linq;

namespace AspNetCoreDemo.Database
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig) 
        { 
            this.databaseConfig = databaseConfig; 
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'Swifts';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "Swifts")
                return;

            connection.Execute("Create Table Swifts (" +
                "BasicHeaderBlock VARCHAR(100) NOT NULL," +
                "ApplicationHeaderBlock VARCHAR(100) NOT NULL," +
                "UserHeaderBlock VARCHAR(100) NULL," +
                "TransactionReferenceNumber VARCHAR(100) NOT NULL," +
                "RelatedReference VARCHAR(100) NULL," +
                "Narrative VARCHAR(1000) NOT NULL," +
                "TrailerBlockMac VARCHAR(100) NOT NULL," +
                "TrailerBlockChk VARCHAR(1000) NOT NULL);");
        }
    }
}
