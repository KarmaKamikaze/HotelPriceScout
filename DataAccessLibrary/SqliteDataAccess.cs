using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DataAccessLibrary
{
    public class SqliteDataAccess : ISqliteDataAccess
    {
        private const string ConnectionString = @"URI=file:../database.sqlite";

        public async Task<IEnumerable<string>> LoadStaticHotelResources(string group)
        {
            using IDbConnection connection = new SQLiteConnection(ConnectionString);
            IEnumerable<string> output = await connection.QueryAsync<string>(
                $"select HotelName from StaticDataHotels WHERE {group.ToLower()} = TRUE", 
                new DynamicParameters());
            IEnumerable<string> resources = output.ToList();

            return resources;
        }

        public async Task<IEnumerable<PriceModel>> RetrieveDataFromDb(string column, string table, string value)
        {

            using IDbConnection connection = new SQLiteConnection(ConnectionString);
            IEnumerable<PriceModel> output = await connection.QueryAsync<PriceModel>(
                $"Select {column} From {table} Where {value}", new DynamicParameters());

            List<PriceModel> resources = output.ToList();

            return resources;

        }

        public async Task<IEnumerable<(string, string, string, Dictionary<string, string>)>> LoadStaticBookingSiteResources()
        {
            using IDbConnection connection = new SQLiteConnection(ConnectionString);
            IEnumerable<(string, string, string, string)> output = 
                await connection.QueryAsync<(string, string, string, string)>(
                    "select * from StaticDataBookingSites", new DynamicParameters());

            var resources = new List<(string, string, string, Dictionary<string, string>)>();

            foreach ((string name, string type, string url, string hotels) item in output)
            {
                var hotelDict = new Dictionary<string, string>();
                string[] hotelNameAndTag = item.hotels.Split(';');
                foreach (var hotelTagPair in hotelNameAndTag)
                {
                    string[] hotelNameAndTagDivided = hotelTagPair.Split(':');
                    hotelDict.Add(hotelNameAndTagDivided[0], hotelNameAndTagDivided[1]);
                }
                resources.Add((item.name, item.type, item.url, hotelDict));
            }

            return resources;
        }

        public async Task SaveToDb<T>(string sqlQuery, T parameters)
        {
            using IDbConnection connection = new SQLiteConnection(ConnectionString);
            await connection.ExecuteAsync(sqlQuery, parameters);
        } 
    }
}
