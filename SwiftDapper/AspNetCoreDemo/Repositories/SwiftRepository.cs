using Dapper;
using Microsoft.Data.Sqlite;
using AspNetCoreDemo.Database;
using AspNetCoreDemo.Repositories.Contracts;
using System.Threading.Tasks;
using AspNetCoreDemo.Models;
using System.ComponentModel;
using System.Collections.Generic;

namespace AspNetCoreDemo.Repositories
{
    public class SwiftRepository : ISwiftRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public SwiftRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<Swift> CreateAsync(Swift swift)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            await connection.ExecuteAsync("INSERT INTO Swifts (" +
                "BasicHeaderBlock, ApplicationHeaderBlock, UserHeaderBlock, TransactionReferenceNumber, RelatedReference, Narrative, TrailerBlockMac, TrailerBlockChk)" +
                "VALUES (@BasicHeaderBlock, @ApplicationHeaderBlock, @UserHeaderBlock, @TransactionReferenceNumber, @RelatedReference, @Narrative, @TrailerBlockMac, @TrailerBlockChk);", swift);

            return swift;
        }

        public async Task<IEnumerable<Swift>> GetAllAsync()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            return await connection.QueryAsync<Swift>("SELECT rowid AS Id, BasicHeaderBlock, ApplicationHeaderBlock, UserHeaderBlock, TransactionReferenceNumber, RelatedReference, Narrative, TrailerBlockMac, TrailerBlockChk FROM Swifts;");
        }
    }
}

