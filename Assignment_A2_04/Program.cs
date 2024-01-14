using Assignment_A2_04.Models;
using Assignment_A2_04.Services;

namespace Assignment_A2_04
{
    class Program
    {


        static void Main(string[] args)
        {
            var service = new NewsService();
            service.NewsAvailable += Service_NewsAvailable;

            Task<News>[] tasks = { null, null, null, null, null, null, null, null, null, null, null, null, null, null };
            
            try
            {
                tasks[0] = service.GetNewsAsync(NewsCategory.business);
                tasks[1] = service.GetNewsAsync(NewsCategory.entertainment);
                tasks[2] = service.GetNewsAsync(NewsCategory.science);
                tasks[3] = service.GetNewsAsync(NewsCategory.general);
                tasks[4] = service.GetNewsAsync(NewsCategory.sports);
                tasks[5] = service.GetNewsAsync(NewsCategory.health);
                tasks[6] = service.GetNewsAsync(NewsCategory.technology);

                Task.WaitAll(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]);

                tasks[7] = service.GetNewsAsync(NewsCategory.business);
                tasks[8] = service.GetNewsAsync(NewsCategory.entertainment);
                tasks[9] = service.GetNewsAsync(NewsCategory.science);
                tasks[10] = service.GetNewsAsync(NewsCategory.health);
                tasks[11] = service.GetNewsAsync(NewsCategory.general);
                tasks[12] = service.GetNewsAsync(NewsCategory.sports);
                tasks[13] = service.GetNewsAsync(NewsCategory.technology);

                Task.WaitAll(tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13]);

                WriteNews(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        private static void Service_NewsAvailable(object sender, string e)
        {
            Console.WriteLine($"{e}");
        }

        private static void WriteNews(Task<News>[] tasks)
        {
            foreach (var task in tasks)
            {
                if (task.IsCompletedSuccessfully)
                {
                    var news = task.Result;


                    Console.WriteLine("---------------------------------");
                    Console.WriteLine($"News in Category {news.Category}");

                    foreach (var item in news.Articles)
                    {
                        Console.WriteLine($"- {item.DateTime}: {item.Title}");
                    }
                }
            }
        }
    }
}
