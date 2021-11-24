# HPS (Hotel Price Scout)

This program is designed to notify hotel price managers of competitor price changes and display the discrepancies in a calendar overview. Additionally, a price thermometer is available to suggest price regulation with the average market in mind.

The software is developed by the P3-Project CS-21-SW-3-12 team.

<!-- GETTING STARTED -->
 ## Getting Started
 
 To get a local copy up and running, follow the steps below.
 
 ### Prerequisites
 
 [.NET SDK 5.0 or later](https://dotnet.microsoft.com/download/dotnet/5.0)
 
 ### Installation 

 1. Clone the repository

```sh
git clone https://github.com/KarmaKamikaze/HotelPriceScout.git
```

 2. Navigate to the root folder of the HotelPriceScout project and run `dotnet restore` to install all dependencies
```sh
cd HotelPriceScout/HotelPriceScout && dotnet restore
```

 3. Create an .xml file called `mail_config.xml` and fill in the nessecary information in the following way:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <MailConfig>
        <SenderEmailAddress>SOFTWARE_EMAIL</SenderEmailAddress>
        <SenderPassword>SOFTWARE_PASSWORD</SenderPassword>
        <ReceiverEmailAddress>RECEIVER_ADDRESS</ReceiverEmailAddress>
        <ReceiverName>RECEIVER_NAME</ReceiverName>
    </MailConfig>
</configuration>
```

 4. While in the root folder, start the server by running `dotnet run`
```sh
dotnet run
```

 5. The site can be visited in a browser at the address `localhost:5001`

```sh
https://localhost:5001
```

### Using the program

FILL IN AFTER DEVELOPMENT HAS FINISHED

 <!-- LICENSE -->
 ## License
 
 Distributed under the GNU General Public License v3.0 License. See `LICENSE` for more information. 


 <!-- CONTACT --> 
 ## Contact 
 
 Project Link: [https://github.com/KarmaKamikaze/HotelPriceScout](https://github.com/KarmaKamikaze/HotelPriceScout)
