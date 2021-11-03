using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace DataAccessLibrary
{
    public class SqliteDataAccess
    {
        private readonly IConfiguration _config;

        public SqliteDataAccess(IConfiguration config)
        {
            _config = config;
        }

        private string LoadConnectionString(string id = "Default")
        {
            return _config.GetConnectionString(id);
        }
        
        public async Task<IEnumerable<string>> LoadStaticHotelResources(string group)
        {
            using IDbConnection connection = new SQLiteConnection(LoadConnectionString());
            IEnumerable<string> output = await connection.QueryAsync<string>($"select * from StaticData WHERE {group.ToLower()} = TRUE", new DynamicParameters());
            IEnumerable<string> resources = output.ToList();

            foreach (var item in resources)
            {
                Console.WriteLine(item);
            }

            return resources;
        }
    }
}