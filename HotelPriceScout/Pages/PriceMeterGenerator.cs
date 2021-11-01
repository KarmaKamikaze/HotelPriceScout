using System.Collections.Generic;


namespace HotelPriceScout.Pages
{
    /*  Caspers mission statement:
     Vaterpasset, skal tage en liste af data som input.
     Med dataen er data for Gns pris for markedet, denne skal være den miderste kolonne. 
     Ud fra dataen skal hotellens navne placeres endten over eller udermarkeds prisen, 
     disse skal oprette nye pris mærker I vaterpasset som er præcis vist I vaterpasset, 
     har 2 hoteller samme pris skal de placeres I samme kolonne og "pege" på samme pris.

     Først skal listen af priser sorteres, Så skal priserne indeles I 2 kategorier, 
     dem der er over og dem der er under markedet, på denne måde kan det tælles hvor mange kasser over og under markedet der skal laves. 
     Dette skal konstrueres som html kode, hvor alt data kommer fra koden, 
     det eneste element der er oprettet statisk er kassen omkring vatterpasset, 
     som er template, samt det data som er generist, som overskrift og grafik.
         */
    public class PriceMeterGenerator // the generator class
    {
        public static List<Prices> PriceListGenerator()
        {
            List<Prices> PriceDataList = new List<Prices>();
            PriceDataList.Add(new Prices("Hotel Kompas", 599));
            PriceDataList.Add(new Prices("Hotel Radison", 999));
            PriceDataList.Add(new Prices("Hotel Phoenix", 499));
            PriceDataList.Add(new Prices("Hotel Zleep", 799));
            PriceDataList.Add(new Prices("Hotel Cabin", 499));
            PriceDataList.Add(new Prices("Gns. Marked", 679));
            PriceDataList.Sort();
            return PriceDataList;
        }
        public static Prices MarketFinder(List<Prices> List)
        {
            Prices MarketPriceItem = null;
            foreach (Prices item in List)
            {
                if (item.Name == "Gns. Marked")
                { MarketPriceItem = item; }
                //Mangler nok en else statement incase der ikke er blevet lavet en market price i databehandlingen.           
             }
            return MarketPriceItem;
        }
    }
}
