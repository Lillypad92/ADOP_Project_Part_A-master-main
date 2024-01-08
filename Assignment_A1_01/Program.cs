using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            double latitude = 60.61667;
            double longitude = 16.76667;

            Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

            //Your Code to present each forecast item in a grouped list
            Console.WriteLine($"Weather forecast for {forecast.City}");

            var groupedByDates = forecast.Items.GroupBy(x => x.DateTime.DayOfYear);

            foreach (var dates in groupedByDates) 
            {

                DateTime dateTimeOfYear = new DateTime(DateTime.Now.Year, 1, 1).AddDays(dates.Key - 1);
              
                Console.WriteLine(dateTimeOfYear.ToString("yyyy-MM-dd"));
                foreach (var date in dates) 
                {
                    Console.WriteLine($"- {date.DateTime.ToString("H:mm")}: {date.Description}, temperatur {date.Temperature} Celsius, vind: {date.WindSpeed} m/s");
                    
                }
            }

        }
    }
}
