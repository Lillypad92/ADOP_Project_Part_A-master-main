using Assignment_A1_03.Models;
using Assignment_A1_03.Services;

namespace Assignment_A1_03
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            //Register the event
            //Your Code
            service.WeatherForecastAvailable += Service_WeatherForecastAvailable;

            Task<Forecast>[] tasks = { null, null, null, null };
            Exception exception = null;
            try
            {
                double latitude = 60.642200;
                double longitude = 16.551070;

                //Create the two tasks and wait for completion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("Orlando");

                Task.WaitAll(tasks[0], tasks[1]);

                tasks[2] = service.GetForecastAsync(latitude, longitude);
                tasks[3] = service.GetForecastAsync("Orlando");

                Task.WaitAll(tasks[2], tasks[3]);
            }
            catch (Exception ex)
            {
                exception = ex;
                //How to handle an exception
                //Your Code
                Console.WriteLine("Weather service error:");
                Console.WriteLine($"Exception occourd: {ex.Message}");
            }
            foreach (var task in tasks)
            {
                //How to deal with successful and fault tasks
                //Your Code
                if (task.IsCompletedSuccessfully)
                {
                    Console.WriteLine();
                    Console.WriteLine("----------------------------------------------");
                    var currentForecast = task.Result;
                    Console.WriteLine($"Weather forecast for {currentForecast.City}");

                    var groupBySandviken = currentForecast.Items.GroupBy(x => x.DateTime.DayOfYear);
                    foreach (var currentDateGroup in groupBySandviken)
                    {
                        DateTime forecastDates = DateTime.Now.AddDays(currentDateGroup.Key - 1);
                        Console.WriteLine(forecastDates.ToString("yyyy-MM-dd"));
                        foreach (var date in currentDateGroup)
                        {
                            Console.WriteLine($"- {date.DateTime.ToString("H:mm")}: {date.Description}, temperatur {date.Temperature} Celsius, vind: {date.WindSpeed} m/s.");
                        }
                    }
                }
                else if (task.IsFaulted)
                {
                    Console.WriteLine($"Task faulted: {task.Exception?.InnerException?.Message}");
                }
            }
        }
        //Event handler declaration
        //Your Code
        private static void Service_WeatherForecastAvailable(object sender, string e)
        {
            Console.WriteLine($"Event message from weather service:{e}");
        }
    }
}
