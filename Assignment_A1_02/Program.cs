using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_02.Models;
using Assignment_A1_02.Services;

namespace Assignment_A1_02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();
            //Register the event
            //Your Code 
            service.WeatherForecastAvailable += Service_WeatherForecastAvailable;
            
            Task<Forecast>[] tasks = { null, null };
            Exception exception = null;
            try
            {
                //Geolocation
                double latitude = 60.642200;
                double longitude = 16.551070;

                Forecast forecastSandviken = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

                //Your Code to present each forecast item in a grouped list
                //Create the two tasks and wait for completion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("Orlando");

                await Task.WhenAll(tasks[0], tasks[1]);

                Task.WaitAll(tasks[0], tasks[1]);
                foreach (var task in tasks)
                {
                    //How to deal with successful and fault tasks
                    //Your Code
                    if (task.IsCompletedSuccessfully)
                    {
                        Console.WriteLine();
                        Console.WriteLine("----------------------------------------------");
                        var currentForecast = await task;
                        Console.WriteLine($"Weather forecast for {currentForecast.City}");

                        var groupedByOrlando = currentForecast.Items.GroupBy(x => x.DateTime.DayOfYear);
                        foreach (var currentDateGroup in groupedByOrlando)
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
            catch (Exception ex)
            {
                exception = ex;
                //How to handle an exception
                //Your Code
                Console.WriteLine("Weather service error:");
                Console.WriteLine($"Exeption occured: {ex.Message}");
            }
        }
        //Event handler declaration
        //Your Code
        private static void Service_WeatherForecastAvailable(object sender, string e)
        {
            Console.WriteLine($"Events recieved {e}");
        }
    }
}
