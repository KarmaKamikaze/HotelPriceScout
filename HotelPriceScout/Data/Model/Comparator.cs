using DataAccessLibrary;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;


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
            await _db.SaveToDB<dynamic>($"CREATE TABLE [{tableName}] ([HotelName] text NOT NULL, [Price] int NOT NULL, [Date] date NOT NULL);", new { });
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
            mail.To.Add(new MailboxAddress("CS-21-SW-3-12", "croska19@student.aau.dk"));
            mail.Subject = "Hotel Price Scout has noticed a price discrepancy!";
            mail.Importance = MessageImportance.High;
            mail.Priority = MessagePriority.Urgent;

            string mailContent = "<p><a href=https://localhost:44317>Go to dashboard!</a></p><p>Here is a list of days where a discrepancy has been noticed:</p>";
            mailContent += "";
            mailContent += "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional //EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
"<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">" +
"<head>" +
"  <!--[if gte mso 9]>" +
"<xml>" +
"  <o:OfficeDocumentSettings>" +
"    <o:AllowPNG/>" +
"    <o:PixelsPerInch>96</o:PixelsPerInch>" +
"  </o:OfficeDocumentSettings>" +
"</xml>" +
"<![endif]-->" +
"  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">" +
"  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
"  <meta name=\"x-apple-disable-message-reformatting\">" +
"  <!--[if !mso]><!-->" +
"  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
"  <!--<![endif]-->" +
"  <title></title>" +
"  <style type=\"text/css\">" +
"    table," +
"    td {" +
"      color: #000000;" +
"    }" +
"    " +
"    a {" +
"      color: #0000ee;" +
"      text-decoration: underline;" +
"    }" +
"    " +
"    @media only screen and (min-width: 620px) {" +
"      .u-row {" +
"        width: 600px !important;" +
"      }" +
"      .u-row .u-col {" +
"        vertical-align: top;" +
"      }" +
"      .u-row .u-col-100 {" +
"        width: 600px !important;" +
"      }" +
"    }" +
"    " +
"    @media (max-width: 620px) {" +
"      .u-row-container {" +
"        max-width: 100% !important;" +
"        padding-left: 0px !important;" +
"        padding-right: 0px !important;" +
"      }" +
"      .u-row .u-col {" +
"        min-width: 320px !important;" +
"        max-width: 100% !important;" +
"        display: block !important;" +
"      }" +
"      .u-row {" +
"        width: calc(100% - 40px) !important;" +
"      }" +
"      .u-col {" +
"        width: 100% !important;" +
"      }" +
"      .u-col>div {" +
"        margin: 0 auto;" +
"      }" +
"    }" +
"    " +
"    body {" +
"      margin: 0;" +
"      padding: 0;" +
"    }" +
"    " +
"    table," +
"    tr," +
"    td {" +
"      vertical-align: top;" +
"      border-collapse: collapse;" +
"    }" +
"    " +
"    p {" +
"      margin: 0;" +
"    }" +
"    " +
"    .ie-container table," +
"    .mso-container table {" +
"      table-layout: fixed;" +
"    }" +
"    " +
"    * {" +
"      line-height: inherit;" +
"    }" +
"    " +
"    a[x-apple-data-detectors='true'] {" +
"      color: inherit !important;" +
"      text-decoration: none !important;" +
"    }" +
"  </style>" +
"  <!--[if !mso]><!-->" +
"  <link href=\"https://fonts.googleapis.com/css?family=Cabin:400,700\" rel=\"stylesheet\" type=\"text/css\">" +
"  <!--<![endif]-->" +
"</head>" +
"<body class=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000\">" +
"  <!--[if IE]><div class=\"ie-container\"><![endif]-->" +
"  <!--[if mso]><div class=\"mso-container\"><![endif]-->" +
"  <table style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%\" cellpadding=\"0\" cellspacing=\"0\">" +
"    <tbody>" +
"      <tr style=\"vertical-align: top\">" +
"        <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">" +
"          <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #f9f9f9;\"><![endif]-->" +
"          <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" +
"            <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">" +
"              <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" +
"                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: transparent;\"><![endif]-->" +
"                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->" +
"                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" +
"                  <div style=\"width: 100% !important;\">" +
"                    <!--[if (!mso)&(!IE)]><!-->" +
"                    <div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">" +
"                      <!--<![endif]-->" +
"                      <!--[if (!mso)&(!IE)]><!-->" +
"                    </div>" +
"                    <!--<![endif]-->" +
"                  </div>" +
"                </div>" +
"                <!--[if (mso)|(IE)]></td><![endif]-->" +
"                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" +
"              </div>" +
"            </div>" +
"          </div>" +
"          <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" +
"            <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">" +
"              <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" +
"                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->" +
"                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->" +
"                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" +
"                  <div style=\"width: 100% !important;\">" +
"                    <!--[if (!mso)&(!IE)]><!-->" +
"                    <div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">" +
"                      <!--<![endif]-->" +
"                      <!--[if (!mso)&(!IE)]><!-->" +
"                    </div>" +
"                    <!--<![endif]-->" +
"                  </div>" +
"                </div>" +
"                <!--[if (mso)|(IE)]></td><![endif]-->" +
"                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" +
"              </div>" +
"            </div>" +
"          </div>" +
"          <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" +
"            <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #003399;\">" +
"              <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" +
"                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #003399;\"><![endif]-->" +
"                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->" +
"                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" +
"                  <div style=\"width: 100% !important;\">" +
"                    <!--[if (!mso)&(!IE)]><!-->" +
"                    <div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">" +
"                      <!--<![endif]-->" +
"                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" +
"                        <tbody>" +
"                          <tr>" +
"                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:0px;font-family:'Cabin',sans-serif;\" align=\"left\">" +
"                              <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" +
"                                <tr>" +
"                                  <td style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">" +
"                                    <img align=\"center\" border=\"0\" src=\"https://s3.amazonaws.com/unroll-images-production/projects%2F47519%2F1636983227779-kompas-logo.png\" alt=\"Image\" title=\"Image\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 600px;\"" +
"                                      width=\"600\" />" +
"                                  </td>" +
"                                </tr>" +
"                              </table>" +
"                            </td>" +
"                          </tr>" +
"                        </tbody>" +
"                      </table>" +
"                      <!--[if (!mso)&(!IE)]><!-->" +
"                    </div>" +
"                    <!--<![endif]-->" +
"                  </div>" +
"                </div>" +
"                <!--[if (mso)|(IE)]></td><![endif]-->" +
"                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" +
"              </div>" +
"            </div>" +
"          </div>" +
"          <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" +
"            <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">" +
"              <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" +
"                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: transparent;\"><![endif]-->" +
"                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->" +
"                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" +
"                  <div style=\"width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">" +
"                    <!--[if (!mso)&(!IE)]><!-->" +
"                    <div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">" +
"                      <!--<![endif]-->" +
"                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" +
"                        <tbody>" +
"                          <tr>" +
"                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">" +
"                              <h1 style=\"margin: 0px; color: #f1c40f; line-height: 140%; text-align: center; word-wrap: break-word; font-weight: normal; font-family: arial,helvetica,sans-serif; font-size: 22px;\">" +
"                                Discrepancy noticed!" +
"                              </h1>" +
"                            </td>" +
"                          </tr>" +
"                        </tbody>" +
"                      </table>" +
"                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" +
"                        <tbody>" +
"                          <tr>" +
"                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">" +
"                              <div style=\"line-height: 140%; text-align: center; word-wrap: break-word;\">" +
"                                <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-family: 'arial black', 'avant garde', arial; font-size: 14px; line-height: 19.6px;\">Please look at the following dates:</span></p>" +
"                              </div>" +
"                            </td>" +
"                          </tr>" +
"                        </tbody>" +
"                      </table>" +
"                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" +
"                        <tbody>" +
"                          <tr>" +
"                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">" +
"                              <div style=\"line-height: 140%; text-align: center; word-wrap: break-word;\">";

            ;
            foreach (MarketPriceModel price in AvgMarketPrices.Where(p => p.MarkedForDiscrepancy && p.Date < DateTime.Now.AddMonths(1)).ToList())
            {
                KeyValuePair<DateTime, Dictionary<string, decimal>> query;
                if (price.RoomType == 1)
                {
                    query = Roomtype1HotelAvgPrices.Single(hp => hp.Key == price.Date);                    
                }
                else if (price.RoomType == 2)
                {
                    query = Roomtype2HotelAvgPrices.Single(hp => hp.Key == price.Date);                   
                }
                else
                {
                    query = Roomtype4HotelAvgPrices.Single(hp => hp.Key == price.Date);                  
                }
                decimal hostprice = query.Value["Kompas Hotel Aalborg"];

                mailContent += $"<p>{price.Date.ToString("d")}: Room type {price.RoomType} Market price {price.Price} Your price {hostprice} </p>";
            }

            mailContent += " </div>" +
"                            </td>" +
"                          </tr>" +
"                        </tbody>" +
"                      </table>" +
"                      <!--[if (!mso)&(!IE)]><!-->" +
"                    </div>" +
"                    <!--<![endif]-->" +
"                  </div>" +
"                </div>" +
"                <!--[if (mso)|(IE)]></td><![endif]-->" +
"                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" +
"              </div>" +
"            </div>" +
"          </div>" +
"          <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" +
"            <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">" +
"              <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" +
"                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->" +
"                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->" +
"                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" +
"                  <div style=\"width: 100% !important;\">" +
"                    <!--[if (!mso)&(!IE)]><!-->" +
"                    <div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">" +
"                      <!--<![endif]-->" +
"                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" +
"                        <tbody>" +
"                          <tr>" +
"                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">" +
"                              <div align=\"center\">" +
"                                <!--[if mso]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-spacing: 0; border-collapse: collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;font-family:'Cabin',sans-serif;\"><tr><td style=\"font-family:'Cabin',sans-serif;\" align=\"center\"><v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" href=\"https://localhost:44317/\" style=\"height:46px; v-text-anchor:middle; width:247px;\" arcsize=\"8.5%\" stroke=\"f\" fillcolor=\"#ff6600\"><w:anchorlock/><center style=\"color:#FFFFFF;font-family:'Cabin',sans-serif;\"><![endif]-->" +
"                                <a href=\"https://localhost:44317/\" target=\"_blank\" style=\"box-sizing: border-box;display: inline-block;font-family:'Cabin',sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #ff6600; border-radius: 4px;-webkit-border-radius: 4px; -moz-border-radius: 4px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;\">" +
"                                  <span style=\"display:block;padding:14px 44px 13px;line-height:120%;\"><span style=\"font-size: 16px; line-height: 19.2px;\"><strong><span style=\"line-height: 19.2px; font-size: 16px;\">Check Dashboard here!</span></strong>" +
"                                  </span>" +
"                                  </span>" +
"                                </a>" +
"                                <!--[if mso]></center></v:roundrect></td></tr></table><![endif]-->" +
"                              </div>" +
"                            </td>" +
"                          </tr>" +
"                        </tbody>" +
"                      </table>" +
"                      <!--[if (!mso)&(!IE)]><!-->" +
"                    </div>" +
"                    <!--<![endif]-->" +
"                  </div>" +
"                </div>" +
"                <!--[if (mso)|(IE)]></td><![endif]-->" +
"                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" +
"              </div>" +
"            </div>" +
"          </div>" +
"          <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" +
"            <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #e5eaf5;\">" +
"              <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" +
"                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #e5eaf5;\"><![endif]-->" +
"                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->" +
"                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" +
"                  <div style=\"width: 100% !important;\">" +
"                    <!--[if (!mso)&(!IE)]><!-->" +
"                    <div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">" +
"                      <!--<![endif]-->" +
"                      <!--[if (!mso)&(!IE)]><!-->" +
"                    </div>" +
"                    <!--<![endif]-->" +
"                  </div>" +
"                </div>" +
"                <!--[if (mso)|(IE)]></td><![endif]-->" +
"                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" +
"              </div>" +
"            </div>" +
"          </div>" +
"          <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" +
"            <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #003399;\">" +
"              <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" +
"                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #003399;\"><![endif]-->" +
"                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->" +
"                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" +
"                  <div style=\"width: 100% !important;\">" +
"                    <!--[if (!mso)&(!IE)]><!-->" +
"                    <div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">" +
"                      <!--<![endif]-->" +
"                      <!--[if (!mso)&(!IE)]><!-->" +
"                    </div>" +
"                    <!--<![endif]-->" +
"                  </div>" +
"                </div>" +
"                <!--[if (mso)|(IE)]></td><![endif]-->" +
"                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" +
"              </div>" +
"            </div>" +
"          </div>" +
"          <!--[if (mso)|(IE)]></td></tr></table><![endif]-->" +
"        </td>" +
"      </tr>" +
"    </tbody>" +
"  </table>" +
"  <!--[if mso]></div><![endif]-->" +
"  <!--[if IE]></div><![endif]-->" +
"</body>" +
"</html>";

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
