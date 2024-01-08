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

                //Create the two tasks and wait for completion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("Orlando");

                await Task.WhenAll(tasks[0], tasks[1]);

                Task.WaitAll(tasks[0], tasks[1]);
            }
            catch (Exception ex)
            {
                exception = ex;
                //How to handle an exception
                //Your Code
                Console.WriteLine($"Exeption occured: {ex.Message}");
            }

            foreach (var task in tasks)
            {
                //How to deal with successful and fault tasks
                //Your Code

                if (task != null) 
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine($"Task faulted: {task.Exception?.InnerException?.Message}");
                    }
                    else if (task.IsCompletedSuccessfully) 
                    {
                        var forecast = await task;
                        Console.WriteLine($"Weather forecast for {forecast.City}");
                    }
                }
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