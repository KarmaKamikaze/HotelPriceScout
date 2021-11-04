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
            IEnumerable<string> output = await connection.QueryAsync<string>(
                $"select HotelName from StaticDataHotels WHERE {group.ToLower()} = TRUE", new DynamicParameters());
            IEnumerable<string> resources = output.ToList();

            return resources;
        }

        public async Task<List<(string, string, string, Dictionary<string, string>)>> LoadStaticBookingSiteResources()
        {
            using IDbConnection connection = new SQLiteConnection(LoadConnectionString());
            IEnumerable<(string name, string type, string url, string hotels)> output = await connection.QueryAsync<(string, string, string, string)>("select * from StaticDataBookingSites", new DynamicParameters());

            var resources = new List<(string, string, string, Dictionary<string, string>)>();

            foreach ((string name, string type, string url, string hotels) item in output)
            {
                var hotelDict = new Dictionary<string, string>();
                string[] hotelNameAndTag = item.hotels.Split(';');
                for (int k = 0; k < hotelNameAndTag.Length; k++)
                {
                    string[] hotelNameAndTagDivided = hotelNameAndTag[k].Split(':');
                    hotelDict.Add(hotelNameAndTagDivided[0], hotelNameAndTagDivided[1]);
                }
                resources.Add((item.name, item.type, item.url, hotelDict));
            }

            return resources;
        }
    }
}