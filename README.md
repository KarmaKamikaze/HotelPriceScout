# HPS (Hotel Price Scout)

This program is designed to notify hotel price managers of competitor price changes and display the discrepancies in a calendar overview. Additionally, a price thermometer is available to suggest price regulation with the average market in mind.

The P3-Project CS-21-SW-3-12 team develops the software.

<!-- GETTING STARTED -->
 ## Getting Started
 
 To get a local copy up and running, follow the steps below.
 
 ### Prerequisites
 
 [.NET SDK 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
 
 ### Installation 

 1. Clone the repository

```sh
git clone https://github.com/KarmaKamikaze/HotelPriceScout.git
```

 2. Navigate to the root folder of the HotelPriceScout project and run `dotnet restore` to install all dependencies
```sh
cd HotelPriceScout/HotelPriceScout && dotnet restore
```

 3. Create an .xml file called `mail_config.xml` and fill in the necessary information in the following way:
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

 <!-- Using the program -->
## Using the program

When the program is opened for the first time, the Settings page (pictured below) will appear.

![Picture of the settings page UI](https://github.com/KarmaKamikaze/HotelPriceScout/blob/dev/.github/images/settingsPageNew.PNG)

The Settings page has the following functionality:

1. Changing the **Margin value percent**. 
   1. This will change the program's sensitivity, i.e., how far the client's prices have to be from the average market price before it is flagged.

2. Changing the **Amount of notifications**. 
   1. Allows the user to choose how many times a day (0-3) to notify of discrepancies between client prices and average market prices.

3. Changing the **Notification time**. 
   1. Allows the user to change the time of day to receive the notification(s) from the program (if any).

4. Start the program with the green button labeled **Start program**. 
   1. Clicking this will trigger a pop-up asking the user to continue to the _Dashboard_ or _stay_ on the Settings page.

5. After the **Start program** button is clicked and the program is running 
   1. The green _Start program_ button will disappear and be replaced with two smaller buttons labeled _Stop program_ and _Update program_. 
   2. _Stop program_ will stop the scouting activities until the program is restarted. 
   3. _Update program_ has to be pressed whenever changes are made to the settings to bring them into effect.

6. The **Dashboard** button in the top-right corner is disabled until the _Start program_ button is pressed. After this, the _Dashboard_ button will redirect to the Dashboard page.

After navigating to the Dashboard page, either by clicking _Go to dashboard_ in the pop-up triggered by starting the program or by clicking the _Dashboard_ button in the top-right corner once the program is started, the interface will appear like the image below:

![Picture of the dashboard page UI](https://github.com/KarmaKamikaze/HotelPriceScout/blob/dev/.github/images/dashboardPageNew.PNG)

The Dashboard page has the following functionality:

1. Two arrow-shaped buttons change the month displayed (Maximum 3 months into the future and 0 into the past).

2. A dropdown menu to choose the room type to show prices for (1/2/4 adult(s)).

3. A dropdown menu to choose specific competing hotels; the average market price will be calculated based on the choices here.

4. An interactive calendar. 
   1. The calendar will show basic information about each day (only the client's price and the average market price). 
   2. To show more in-depth information, click on a day, and a new container will appear on the right side of the screen, including the prices of all selected competitors.

5. The **Scout settings** button in the top right corner. 
   1. Will redirect to the Settings page.

 <!-- LICENSE -->
 ## License
 
 Distributed under the GNU General Public License v3.0 License. See `LICENSE` for more information. 


 <!-- CONTACT --> 
 ## Contact 
 
 Project Link: [https://github.com/KarmaKamikaze/HotelPriceScout](https://github.com/KarmaKamikaze/HotelPriceScout)
