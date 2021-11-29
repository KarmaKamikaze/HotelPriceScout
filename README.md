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

 <!-- Using the program -->
## Using the program

When the program is opened for the first time the Settings page (pictured below) will appear.

![Optional Text](https://github.com/KarmaKamikaze/HotelPriceScout/blob/dev/.github/images/settingsPage.PNG)

The Settings page has the following functionality:

1. Changing the "Margin value percent". This will change the sensitivity of the program i.e. how far the client's prices has to be from the average market price before it is flagged.

2. Changing the "Amount of notifications". Allows the user to choose how many times a day (0-3) to notified of discrepencies between client prices and average market prices.

3. Changing the "Notification time". Allows the user to change the time of day to receive the notification(s) from the program (if any).

4. Starting the program with the green button labeled "Start program". Clicking this will trigger a pop-up asking the user whether to continue to the "Dashboard" or "stay" on the Settings page.

5. [After the "Start program" button is clicked and the program is running] The green "Start program" will disappear and be replaced with two smaller buttons labeled "Stop program" and "Update program". "Stop program" will stop the scouting activities until the program is restarted. "Update program" has to be pressed whenever changes are made to the settings to bring tham into effect.

6. The "Home" button in the top-right corner is disabled until the the "Start program" button is pressed. After this the "Home" button will redirect to the Dashboard page.

After navigating to the Dashboard page, either by clicking "Go to dashboard" in the pop-up triggered by starting the program or by clicking the "Home" button in the top-right corner once the program is started, the interface will appear like the image below:

![Optional Text](https://github.com/KarmaKamikaze/HotelPriceScout/blob/dev/.github/images/dashboardPage.PNG)

The Dashboard page has the following functionality:

1. Two arrow-shaped buttons used to change the month displayed (Maximum 3 months into the future and 0 into the past).

2. A dropdown menu to choose the type of room to show prices for (1/2/4 adult(s)).

3. A dropdown menu to choose specific competing hotels, the average market price will be calculated based on the choises here.

4. An interactive calendar. The calendar will show basic information about each day (only the client's price and the average market price). To show more in-depth information, click on a day and a new container will appear on the right side of the screen, including the prices of all selected competitors.

5. The "Scout settings" button in the top right corner. Will redirect to the Settings page.

 <!-- LICENSE -->
 ## License
 
 Distributed under the GNU General Public License v3.0 License. See `LICENSE` for more information. 


 <!-- CONTACT --> 
 ## Contact 
 
 Project Link: [https://github.com/KarmaKamikaze/HotelPriceScout](https://github.com/KarmaKamikaze/HotelPriceScout)
