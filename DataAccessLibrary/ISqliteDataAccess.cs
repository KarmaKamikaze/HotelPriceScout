using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public interface ISqliteDataAccess
    {
        Task<IEnumerable<string>> LoadStaticHotelResources(string group);
        Task<IEnumerable<PriceModel>> RetrieveDataFromDb(string column, string table, string value);
        Task<IEnumerable<(string, string, string, Dictionary<string, string>)>> LoadStaticBookingSiteResources();
        Task SaveToDb<T>(string sqlQuery, T parameters);
    }
}