using DataAccessLibrary;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Abstractions;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HotelPriceScout.Data.Model
{
    public class Comparator : IComparator
    {
        private readonly SqliteDataAccess _db = new SqliteDataAccess();

        public bool IsDiscrepancy { get; private set; }

        private Dictionary<DateTime, Dictionary<string, decimal>> RoomType1HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> RoomType2HotelAvgPrices { get; set; }
        private Dictionary<DateTime, Dictionary<string, decimal>> RoomType4HotelAvgPrices { get; set; }
        private List<PriceModel> AvgMarketPrices { get; set; }

        public async void ComparePrices(IEnumerable<BookingSite> bookingSites, int marginValue)
        {
            RoomType1HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            RoomType2HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            RoomType4HotelAvgPrices = new Dictionary<DateTime, Dictionary<string, decimal>>();
            AvgMarketPrices = new List<PriceModel>();
            
            DateTime latestScrapedDate = bookingSites.First().HotelsList.First().RoomTypes.First().Prices.Last().Date;

            //For loop that iterates through each date that have been scraped
            for (DateTime date = DateTime.Now.Date; date <= latestScrapedDate; date = date.AddDays(1))
            {
                //These dictionaries describe the values of the key-value-pairs of the RoomTypeXHotelAvgPrices dictionaries.
                Dictionary<string, decimal> dict1 = new();
                Dictionary<string, decimal> dict2 = new();
                Dictionary<string, decimal> dict4 = new();

                //The integer describes the capacity of the roomTypes.
                List<(Dictionary<string, decimal>, int)> dictList = new()
                {
                    (dict1, 1),
                    (dict2, 2),
                    (dict4, 4)
                };

                foreach (BookingSite bookingSite in bookingSites)
                {
                    foreach (Hotel hotel in bookingSite.HotelsList)
                    {
                        //Foreach loop that iterates through the different dictionaries which each are coupled to a roomType 
                        foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                        {
                            RoomType roomType = hotel.RoomTypes.Single(r => r.Capacity == capacity);

                            RoomTypePrice roomTypePrice = roomType.Prices.FirstOrDefault(p => p.Date == date);

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

                RoomType1HotelAvgPrices.Add(date, dict1);
                RoomType2HotelAvgPrices.Add(date, dict2);
                RoomType4HotelAvgPrices.Add(date, dict4);

                foreach ((Dictionary<string, decimal> dict, int capacity) in dictList)
                {
                    AvgMarketPrices.Add(new PriceModel(dict.Values.Average(), date, capacity));
                }
            }

            //Notification should only be sent for discrepancies in the next month
            DateTime earliestNotificationDate = AvgMarketPrices.Min(price => price.Date);
            DateTime latestNotificationDate = earliestNotificationDate.AddMonths(1);
            for (DateTime date = earliestNotificationDate; date < latestNotificationDate; date = date.AddDays(1))
            {
                CheckDiscrepancy(date, RoomType1HotelAvgPrices, marginValue, 1);
                CheckDiscrepancy(date, RoomType2HotelAvgPrices, marginValue, 2);
                CheckDiscrepancy(date, RoomType4HotelAvgPrices, marginValue, 4);
            }

            await StoreAvgHotelPrices(RoomType1HotelAvgPrices, "RoomType1");
            await StoreAvgHotelPrices(RoomType2HotelAvgPrices, "RoomType2");
            await StoreAvgHotelPrices(RoomType4HotelAvgPrices, "RoomType4");
        }

        private async Task StoreAvgHotelPrices(Dictionary<DateTime, Dictionary<string, decimal>> roomTypeHotelAvgPrices,
            string tableName)
        {
            string valueDB = $"INSERT INTO {tableName} (Date,HotelName,Price) VALUES ";
            foreach (KeyValuePair<DateTime, Dictionary<string, decimal>> dateHotelsPair in roomTypeHotelAvgPrices)
            {
                foreach (KeyValuePair<string, decimal> hotelPricePair in dateHotelsPair.Value)
                {
                    valueDB += $"('{dateHotelsPair.Key.ToString("yyyy-MM-dd")}','{hotelPricePair.Key}','{hotelPricePair.Value}'),";
                }
            }

            valueDB = valueDB.TrimEnd(',');
            valueDB += ";";

            await _db.SaveToDB<dynamic>($"DROP TABLE IF EXISTS {tableName};", new { });
            await _db.SaveToDB<dynamic>(
                $"CREATE TABLE [{tableName}] ([HotelName] text NOT NULL, [Price] decimal NOT NULL, [Date] date NOT NULL);",
                new { });
            await _db.SaveToDB<dynamic>(valueDB, new { });
        }

        private void CheckDiscrepancy(DateTime date, Dictionary<DateTime, Dictionary<string, decimal>> hotelAvgPrices,
            decimal marginValue, int capacity)
        {
            PriceModel avgMarketPrice = AvgMarketPrices.Where(price => price.Date == date)
                .Single(price => price.RoomType == capacity);
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
            XDocument mailConfig = XDocument.Load("./mail_config.xml");

            mail.From.Add(new MailboxAddress("Hotel Price Scout", 
                mailConfig.Descendants("SenderEmailAddress").First().Value));
            mail.To.Add(new MailboxAddress(mailConfig.Descendants("ReceiverName").First().Value, 
                mailConfig.Descendants("ReceiverEmailAddress").First().Value));
            mail.Subject = "Hotel Price Scout has noticed a price discrepancy!";
            mail.Importance = MessageImportance.High;
            mail.Priority = MessagePriority.Urgent;

            string tempMail = File.ReadAllText("Data/Model/Mail_strings/mailTemplate.txt");
            string startOfMail = tempMail.Split("SPLIT HERE")[0];
            string endOfMail = tempMail.Split("SPLIT HERE")[1];
            string mailContent = startOfMail;

            string roomType1Mail = "", roomType2Mail = "", roomType4Mail = "";

            foreach (PriceModel price in AvgMarketPrices
                .Where(p => p.MarkedForDiscrepancy && p.Date < DateTime.Now.AddMonths(1)).ToList())
            {
                switch (price.RoomType)
                {
                    case 1:
                        roomType1Mail = MailDataBuilder(RoomType1HotelAvgPrices, roomType1Mail, price);
                        break;
                    case 2:
                        roomType2Mail = MailDataBuilder(RoomType2HotelAvgPrices, roomType2Mail, price);
                        break;
                    default:
                        roomType4Mail = MailDataBuilder(RoomType4HotelAvgPrices, roomType4Mail, price);
                        break;
                }
            }

            if (roomType1Mail != "")
            {
                mailContent += MailHeadBuilder("Roomtype 1", roomType1Mail);
            }

            if (roomType2Mail != "")
            {
                mailContent += MailHeadBuilder("Roomtype 2", roomType2Mail);
            }

            if (roomType4Mail != "")
            {
                mailContent += MailHeadBuilder("Roomtype 4", roomType4Mail);
            }

            mailContent += endOfMail;
            mail.Body = new TextPart(TextFormat.Html)
            {
                Text = mailContent
            };

            SmtpClient smtpClient = new();
            smtpClient.Connect("smtp.gmail.com", 465, true);
            smtpClient.Authenticate(mailConfig.Descendants("SenderEmailAddress").First().Value, 
                mailConfig.Descendants("SenderPassword").First().Value);
            smtpClient.Send(mail);
            smtpClient.Disconnect(true);
        }
        
        public IEnumerable<PriceModel> OneMonthSelectedHotelsMarketPrices(DateTime startDate, DateTime endDate, IEnumerable<PriceModel> dataList)
        {
            if (endDate > startDate.AddMonths(3))
            {
                endDate = startDate.AddMonths(3);
            }
            List<decimal> tempList = new();
            List<PriceModel> listOfSingleDatePrices = new();
            for (DateTime tempDate = startDate; tempDate <= endDate; tempDate = tempDate.AddDays(1))
            {
                tempList.AddRange(from item in dataList
                    where item.Date == tempDate
                    select item.Price);
                PriceModel singleDayMarketPrice = new PriceModel(tempList.Average(), tempDate);
                tempList.Clear();
                listOfSingleDatePrices.Add(singleDayMarketPrice);
            }
            dataList = listOfSingleDatePrices;
            
            return dataList;
        }
        
        private string MailDataBuilder(Dictionary<DateTime, Dictionary<string, decimal>> hotelAvgPrices,
            string containerString, PriceModel price)
        {
            KeyValuePair<DateTime, Dictionary<string, decimal>> query;
            query = hotelAvgPrices.Single(hp => hp.Key == price.Date);
            decimal hostPrice = query.Value["Kompas Hotel Aalborg"];
            containerString +=
                $"<tr><td style='text-align:center'>{price.Date.ToString("d")}</td><td style='text-align:center'>" +
                $"{price.Price.ToString("c0", new CultureInfo("da-DK"))}</td>";
            if (price.Price > hostPrice)
            {
                containerString += "<td style='text-align:center; background-color: #39a459;'>" +
                                   $"{hostPrice.ToString("c0", new CultureInfo("da-DK"))}</td></tr>";
            }
            else
            {
                containerString += $"<td style='text-align:center; background-color: #fc4119;'>" +
                                   $"{hostPrice.ToString("c0", new CultureInfo("da-DK"))}</td></tr>";
            }

            return containerString;
        }

        private string MailHeadBuilder(string roomType, string containerString)
        {
            string result = $"<p><b>{roomType}</p></b>" +
                            "<table><thead><tr><th> Date </th><th> Market Price </th><th> Your Price </th></tr></thead>" +
                            containerString + "</table><br>";
            return result;
        }
    }
}