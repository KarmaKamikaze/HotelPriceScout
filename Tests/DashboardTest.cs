using System;
using Xunit;
using HotelPriceScout.Data.Interface;
using HotelPriceScout.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class DashboardTest
    {
        private static readonly List<string> listOfHotels = new List<string>()
        {
            "Cabinn Aalborg",
            "Slotshotellet Aalborg",
            "Kompas Hotel Aalborg",
            "Aalborg Airport Hotel",
            "Comwell Hvide Hus Aalborg",
            "Helnan Phønix Hotel",
            "Hotel Jomfru Ane",
            "Hotel Scheelsminde",
            "Milling Hotel Aalborg",
            "Prinsen Hotel",
            "Radisson Blu Limfjord Hotel Aalborg",
            "Room Rent Prinsen",
            "Scandic Aalborg Øst",
            "Scandic Aalborg City",
            "Zleep Hotel Aalborg"
        };

        private static readonly List<string> localList = new List<string>()
        {
            "Cabinn Aalborg",
            "Slotshotellet Aalborg",
            "Kompas Hotel Aalborg"
        };

        private static readonly List<string> noBudgetList = new List<string>()
        {
            "Slotshotellet Aalborg",
            "Kompas Hotel Aalborg",
            "Milling Hotel Aalborg",
            "Aalborg Airport Hotel",
            "Helnan Phønix Hotel",
            "Hotel Scheelsminde",
            "Radisson Blu Limfjord Hotel Aalborg",
            "Comwell Hvide Hus Aalborg",
            "Scandic Aalborg Øst",
            "Scandic Aalborg City"
        };

        public static IEnumerable<object[]> FilterOptions =>
            new List<object[]>
            {
                new object[] {"All", listOfHotels},
                new object[] {"Local", localList},
                new object[] {"No budget", noBudgetList}
            };
        
        public static IEnumerable<object[]> GetValues =>
            new List<object[]>
            {
                new object[] {DateTime.Now.Date.ToString()},
                new object[] {"hotel"},
                new object[] {"1"}
            };

        [Fact]
        public void Test_If_CreateMonth_Creates_Correct_Month_Based_On_The_Current_Month()
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            dashboard.CreateMonth();
            //Assert
            Assert.Equal(DateTime.Now.Month, dashboard.Month);
        }

        [Theory]
        [InlineData("low", 100, 1)]
        [InlineData("high", 1, 100)]
        [InlineData("", 0, 0)]
        public void Test_If_ChangeTextColorBasedOnMargin_Returns_Correct_Expected_Value(string expected,
            int marketPrice, int kompasPrice)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            string actual = dashboard.ChangeTextColorBasedOnMargin(marketPrice, kompasPrice);
            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("oi oi-caret-top", 100, 1)]
        [InlineData("oi oi-caret-bottom", 1, 100)]
        [InlineData("oi oi-minus", 0, 0)]
        public void Test_If_ArrowDecider_Returns_Correct_Value(string expected, int marketPrice, int kompasPrice)
        {
            //Arrange
            Dashboard dashboard = new Dashboard();
            //Act
            string actual = dashboard.ArrowDecider(marketPrice, kompasPrice);
            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(GetValues))]
        public void Test_If_UpdateUiMissingData_Returns_Correct_Values_In_WarningMessage(string expectedSubstring)
        {
            Dashboard dashboard = new Dashboard();
            Dictionary<string, string> hotelStrings = new Dictionary<string, string>()
            {
                {"hotel", "tag"}
            };
            BookingSite bookingSite = new BookingSite("bookingSite", "single", "https://www.url.com", hotelStrings);

            bookingSite.DataScraper.StartScraping(10);

            bookingSite.HotelsList.First().RoomTypes.First().Prices.First().Price = 0;

            dashboard.UpdateUiMissingDataWarning(bookingSite);

            string actual = dashboard.WarningMessages.First().ConcatenatedWarningString;

            Assert.Contains(expectedSubstring, actual);
        }

        [Theory]
        [InlineData("All")]
        [InlineData("Local")]
        [InlineData("No budget")]
        [InlineData("Cabinn Aalborg")]
        [InlineData("Hotel")]
        public void SelectedHotelsChangedAddsHotelOptionToList(string option)
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();

            //Act
            dashboard.SelectedHotelsChanged(option);
            
            //Assert
            Assert.Contains(option, dashboard.SelectedHotels);
        }

        [Theory]
        [InlineData("All")]
        [InlineData("Local")]
        [InlineData("No budget")]
        [InlineData("Cabinn Aalborg")]
        [InlineData("Hotel")]
        public void SelectedHotelsChangedRemovesHotelOptionWhenAlreadyInList(string option)
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();

            //Act
            dashboard.SelectedHotelsChanged(option);
            dashboard.SelectedHotelsChanged(option);
            
            //Assert
            Assert.DoesNotContain(option, dashboard.SelectedHotels);
        }
        
        [Theory]
        [MemberData(nameof(FilterOptions))]
        public void SelectedHotelsChangedAddsRelevantHotelsWhenFilterOptionIsSelected(string option, List<string> expectedList)
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();

            //Act
            dashboard.SelectedHotelsChanged(option);
            
            //Assert
            Assert.All(expectedList, hotel => Assert.Contains(hotel, dashboard.SelectedHotels));
        }

        [Theory]
        [InlineData("All")]
        [InlineData("Local")]
        [InlineData("No budget")]
        public void SelectedHotelsChangedUnselectingFilterOptionUnselectsRelevantOptions(string filterOption)
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();
            //Act
            dashboard.SelectedHotelsChanged(filterOption);
            dashboard.SelectedHotelsChanged(filterOption);
            
            //Assert
            Assert.Empty(dashboard.SelectedHotels);
        }

        [Theory]
        [InlineData("Local", "No budget")]
        [InlineData("No budget", "Local")]
        public void
            SelectedHotelsChangedUnselectingFilterOptionDoesNotUnselectSharedHotelsWhenOtherFiltersAreSelected(string filterToRemove, string filterToKeep)
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();
            //Act
            dashboard.SelectedHotelsChanged(filterToRemove);
            dashboard.SelectedHotelsChanged(filterToKeep);
            dashboard.SelectedHotelsChanged(filterToRemove);
            
            //Assert
            bool sharedHotelsNotRemoved = dashboard.SelectedHotels.Contains("Kompas Hotel Aalborg") &&
                                          dashboard.SelectedHotels.Contains("Slotshotellet Aalborg");
            Assert.True(sharedHotelsNotRemoved);
        }

        [Theory]
        [MemberData(nameof(FilterOptions))]
        public void SelectedHotelsChangedAutomaticallyAddsFilterOptionsWhenRelevantHotelsAreAdded(string expectedFilter, List<string> hotels)
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();
            
            //Act
            foreach (string hotel in hotels)
            {
                dashboard.SelectedHotelsChanged(hotel);
            }
            
            //Assert
            Assert.Contains(expectedFilter, dashboard.SelectedHotels);
        }
        
        [Theory]
        [MemberData(nameof(FilterOptions))]
        public void SelectedHotelsChangedAutomaticallyRemovesFilterOptionsWhenRelevantHotelsAreNotInList(string filter, List<string> filterHotels)
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();
            
            //Act
            dashboard.SelectedHotelsChanged(filter);
            dashboard.SelectedHotelsChanged(filterHotels.First());
            
            //Assert
            Assert.DoesNotContain(filter, dashboard.SelectedHotels);
        }
        
        [Fact]
        public void SelectedHotelsIsAlwaysDistinctAfterAddingHotels()
        {
            //Arrange
            IDashboard dashboard = SetupDashboard();
            
            //Act
            dashboard.SelectedHotelsChanged("Local");
            dashboard.SelectedHotelsChanged("No budget");
            dashboard.SelectedHotelsChanged("All");
            
            //Assert
            bool isDistinct = dashboard.SelectedHotels.Count == dashboard.SelectedHotels.Distinct().Count();
            Assert.True(isDistinct);
        }

        private static IDashboard SetupDashboard()
        {
            return new Dashboard
            {
                ListOfHotels = listOfHotels,
                LocalList = localList,
                NoBudgetList = noBudgetList
            };
        }
    }
    
    
    
}
