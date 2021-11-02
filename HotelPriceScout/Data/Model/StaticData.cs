using System;
using System.Collections.Generic;
using System.IO;

namespace HotelPriceScout.Data.Model
{
    public static class StaticData
    {
        public static IEnumerable<string> GetStaticHotelResources(string filterType)
        {
            try
            {
                List<string> hotels = new List<string>();
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Data/Model/HotelResources.csv"));
                Console.WriteLine(path);
                string[] fileResources = File.ReadAllLines(path);

                foreach (string type in fileResources)
                {
                    string[] hotelsList = type.Split(";");
                    if (hotelsList[0] == filterType.ToLower())
                    {
                        for (int i = 1; i < hotelsList.Length; i++)
                        {
                            hotels.Add(hotelsList[i]);
                        }
                    }
                }
                
                return hotels;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}