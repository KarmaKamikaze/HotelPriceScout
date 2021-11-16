using DataAccessLibrary;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace HotelPriceScout.Data.Model
{

    public class Comparator
    {
        private SqliteDataAccess _db = new SqliteDataAccess();

        public Comparator()
        {
            Roomtype1HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            Roomtype2HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            Roomtype4HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            AvgMarketPrices = new List<MarketPriceModel>();
        }

        public bool IsDiscrepancy { get; private set; }

        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype1HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype2HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> Roomtype4HotelAvgPrices { get; set; }
        private List<MarketPriceModel> AvgMarketPrices { get; set; }

        public async void ComparePrices(IEnumerable<BookingSite> bookingSites, int marginValue)
        {
            DateTime LatestScrapedDate = bookingSites.First().HotelsList.First().RoomTypes.First().Prices.Last().Date;

            for (DateTime date = DateTime.Now.Date; date <= LatestScrapedDate; date = date.AddDays(1))
            {
                //These dictionaries describe the values of the key-value-pairs of the RoomtypeXHotelAvgPrices dictionaries.
                Dictionary<string, decimal> dict1 = new();
                Dictionary<string, decimal> dict2 = new();
                Dictionary<string, decimal> dict4 = new();

                //The integer describes the capacity of the roomtypes.
                List<(Dictionary<string, decimal>, int)> dictList = new();
                dictList.Add((dict1, 1));
                dictList.Add((dict2, 2));
                dictList.Add((dict4, 4));

                foreach (BookingSite bookingSite in bookingSites)
                {
                    foreach (Hotel hotel in bookingSite.HotelsList)
                    {
                        foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                        {
                            RoomType roomType = hotel.RoomTypes.Single(r => r.Capacity == capacity);

                            RoomTypePrice roomTypePrice = roomType.Prices.Where(p => p.Date == date).FirstOrDefault();

                            if (roomTypePrice != null)
                            {
                                if (dict.ContainsKey(hotel.Name))
                                {
                                    dict[hotel.Name] = (dict[hotel.Name] + roomTypePrice.Price) / 2;
                                }
                                else
                                {
                                    dict.Add(hotel.Name, roomTypePrice.Price);
                                }
                            }
                        }
                    }
                }

                Roomtype1HotelAvgPrices.Add(date, dict1);
                Roomtype2HotelAvgPrices.Add(date, dict2);
                Roomtype4HotelAvgPrices.Add(date, dict4);

                foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                {
                    AvgMarketPrices.Add(new MarketPriceModel(decimal.ToInt32(dict.Values.Average()), date, capacity));
                }
            }

            DateTime earliestNotifactionDate = AvgMarketPrices.Min(price => price.Date);
            DateTime latestNotificationDate = earliestNotifactionDate.AddMonths(1);
            for (DateTime date = earliestNotifactionDate; date < latestNotificationDate; date = date.AddDays(1))
            {
                CheckDiscrepancy(date, Roomtype1HotelAvgPrices, marginValue, 1);
                CheckDiscrepancy(date, Roomtype2HotelAvgPrices, marginValue, 2);
                CheckDiscrepancy(date, Roomtype4HotelAvgPrices, marginValue, 4);               
            }

            StoreAvgHotelPrices(Roomtype1HotelAvgPrices, "RoomType1");
            StoreAvgHotelPrices(Roomtype2HotelAvgPrices, "RoomType2");
            StoreAvgHotelPrices(Roomtype4HotelAvgPrices, "RoomType4");

            string valueDB = $"INSERT INTO MarketPrices (Date,Price,RoomType) VALUES ";
            foreach (MarketPriceModel marketPrice in AvgMarketPrices)
            {               
                valueDB += $"('{marketPrice.Date.ToString("yyyy-MM-dd")}','{decimal.ToInt32(marketPrice.Price)}','{marketPrice.RoomType}'),";
            }

            valueDB = valueDB.TrimEnd(',');
            valueDB += ";";

            await _db.SaveToDB<dynamic>($"DROP TABLE IF EXISTS MarketPrices;", new { });
            await _db.SaveToDB<dynamic>($"CREATE TABLE [MarketPrices] ([Date] date NOT NULL, [Price] int NOT NULL, [RoomType] int NOT NULL);", new { });
            await _db.SaveToDB<dynamic>(valueDB, new { });

        }

        public async void StoreAvgHotelPrices(Dictionary<DateTime, Dictionary<string, decimal>> roomtypeHotelAvgPrices, string tableName)
        {
            string valueDB = $"INSERT INTO {tableName} (Date,HotelName,Price) VALUES ";
            foreach (KeyValuePair<DateTime, Dictionary<string, decimal>> dateHotelsPair in roomtypeHotelAvgPrices)
            {
                foreach (KeyValuePair<string, decimal> hotelPricePair in dateHotelsPair.Value)
                {
                    valueDB += $"('{dateHotelsPair.Key.ToString("yyyy-MM-dd")}','{hotelPricePair.Key}','{decimal.ToInt32(hotelPricePair.Value)}'),";
                }
            }
            valueDB = valueDB.TrimEnd(',');
            valueDB += ";";

            await _db.SaveToDB<dynamic>($"DROP TABLE IF EXISTS {tableName};", new { });
            await _db.SaveToDB<dynamic>($"CREATE TABLE [{tableName}] ([HotelName] ttext NOT NULL, [Price] int NOT NULL, [Date] date NOT NULL);", new { });
            await _db.SaveToDB<dynamic>(valueDB, new { });
        }


        private void CheckDiscrepancy(DateTime date, Dictionary<DateTime, Dictionary<string, decimal>> hotelAvgPrices, decimal marginValue, int capacity)
        {
            MarketPriceModel avgMarketPrice = AvgMarketPrices.Where(price => price.Date == date).Single(price => price.RoomType == capacity);
            if (hotelAvgPrices[date]["Kompas Hotel Aalborg"] < ((1 - marginValue / 100) * avgMarketPrice.Price) ||
                hotelAvgPrices[date]["Kompas Hotel Aalborg"] > ((1 + marginValue / 100) * avgMarketPrice.Price))
            {
                IsDiscrepancy = true;
                avgMarketPrice.MarkedForDiscrepancy = true;
            }
        }

        public void SendNotification()
        {
            MimeMessage mail = new();
            mail.From.Add(new MailboxAddress("Hotel Price Scout", "hotelpricescout@gmail.com"));
            mail.To.Add(new MailboxAddress("CS-21-SW-3-12", "croska19@student.aau.dk")); /*cs - 21 - sw - 3 - 12@student.aau.dk*/
            mail.Subject = "Hotel Price Scout has noticed a price discrepancy!";
            mail.Importance = MessageImportance.High;
            mail.Priority = MessagePriority.Urgent;

            string startOfMail = File.ReadAllText(@"..\HotelPriceScout\Data\Model\Mail_strings\Top_Mail.txt");
            string mailContent = startOfMail;

            string roomType1Mail = "";
            string roomType2Mail = "";
            string roomType4Mail = "";


            foreach (MarketPriceModel price in AvgMarketPrices.Where(p => p.MarkedForDiscrepancy && p.Date < DateTime.Now.AddMonths(1)).ToList())
            {
                KeyValuePair<DateTime, Dictionary<string, decimal>> query;
                if (price.RoomType == 1)
                {
                    query = Roomtype1HotelAvgPrices.Single(hp => hp.Key == price.Date);
                    decimal hostprice = query.Value["Kompas Hotel Aalborg"];
                    roomType1Mail += $"<tr><td style='text-align:center'>{price.Date.ToString("d")}</td><td style='text-align:center'>{price.Price},-</td>";
                    if (price.Price > hostprice)
                    { roomType1Mail+= $"<td style='text-align:center; background-color: #39a459;'>{hostprice},-</td></tr>"; }
                    else { roomType1Mail += $"<td style='text-align:center; background-color: #fc4119;'>{hostprice},-</td></tr>"; }
                }
                else if (price.RoomType == 2)
                {
                    query = Roomtype2HotelAvgPrices.Single(hp => hp.Key == price.Date);
                    decimal hostprice = query.Value["Kompas Hotel Aalborg"];
                    roomType2Mail += $"<tr><td style='text-align:center'>{price.Date.ToString("d")}</td><td style='text-align:center'>{price.Price},-</td>";
                    if (price.Price > hostprice)
                    { roomType2Mail += $"<td style='text-align:center; background-color: #39a459;'>{hostprice},-</td></tr>"; }
                    else { roomType2Mail += $"<td style='text-align:center; background-color: #fc4119;'>{hostprice},-</td></tr>"; }
                }
                else
                {
                    query = Roomtype4HotelAvgPrices.Single(hp => hp.Key == price.Date);
                    decimal hostprice = query.Value["Kompas Hotel Aalborg"];
                    roomType4Mail += $"<tr><td style='text-align:center'>{price.Date.ToString("d")}</td><td style='text-align:center'>{price.Price},-</td>";
                    if (price.Price > hostprice)
                    { roomType4Mail += $"<td style='text-align:center; background-color: #39a459;'>{hostprice},-</td></tr>"; }
                    else { roomType4Mail += $"<td style='text-align:center; background-color: #fc4119;'>{hostprice},-</td></tr>"; }
                }
            }
            if (roomType1Mail != "") {
                mailContent += "<p><b>Roomtype 1</p></b>" +
                "<table><thead><tr><th> Date </th><th> Market Price </th><th> Your Price </th></tr></thead>" +
                 roomType1Mail +
                 "</table><br>";
            }
            if (roomType2Mail != "")
            {
                mailContent += "<p><b>Roomtype 2</p></b>" +
                "<table><thead><tr><th> Date </th><th> Market Price </th><th> Your Price </th></tr></thead>" +
                 roomType2Mail +
                 "</table><br>";
            }
            if (roomType4Mail != "")
            {
                mailContent += "<p><b>Roomtype 4</p></b>" +
                "<table><thead><tr><th> Date </th><th> Market Price </th><th> Your Price </th></tr></thead>" +
                 roomType4Mail +
                 "</table><br>";
            }
            string endOfMail = File.ReadAllText(@"..\HotelPriceScout\Data\Model\Mail_strings\Bottom_Mail.txt");
            mailContent += endOfMail;

            mail.Body = new TextPart(TextFormat.Html)
            {
                Text = mailContent
            };

            SmtpClient smtpClient = new();
            smtpClient.Connect("smtp.gmail.com", 465, true);
            smtpClient.Authenticate("hotelpricescout@gmail.com", "cs-21-sw-3-12");
            smtpClient.Send(mail);
            smtpClient.Disconnect(true);
            
            
        }

    }





}
