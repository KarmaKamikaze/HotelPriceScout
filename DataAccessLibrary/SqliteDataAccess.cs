using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using HotelPriceScout.Data.Model;

namespace DataAccessLibrary
{
    public class SqliteDataAccess
    {

        private readonly IConfiguration _config;

        private static string connectionString = @"URI=file:../database.sqlite";

        
        public async Task<IEnumerable<string>> LoadStaticHotelResources(string group)
        {
            using IDbConnection connection = new SQLiteConnection(connectionString);
            IEnumerable<string> output = await connection.QueryAsync<string>(
                $"select HotelName from StaticDataHotels WHERE {group.ToLower()} = TRUE", new DynamicParameters());
            IEnumerable<string> resources = output.ToList();

            return resources;
        }

        public async Task<IEnumerable<MarketPriceModel>> RetrieveDataFromDb(string column, string table, string value)
        {

            using IDbConnection connection = new SQLiteConnection(connectionString);
            IEnumerable<MarketPriceModel> output = await connection.QueryAsync<MarketPriceModel>($"Select {column} From {table} Where {value}", new DynamicParameters());

            List<MarketPriceModel> resources = output.ToList();

            return resources;

        }

        public async Task<IEnumerable<(string, string, string, Dictionary<string, string>)>> LoadStaticBookingSiteResources()
        {
            using IDbConnection connection = new SQLiteConnection(connectionString);
            IEnumerable<(string, string, string, string)> output = await connection.QueryAsync<(string, string, string, string)>("select * from StaticDataBookingSites", new DynamicParameters());

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
        //public async Task SaveData<T>(string sql, T parameters)
        //{
           
        //    using (IDbConnection connection = new SQLiteConnection(connectionString))
        //    { 
        //        await connection.ExecuteAsync(sql, parameters); 
        //    }
                

           
        //}
        

    }
}