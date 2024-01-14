using Assignment_A2_02.Models;
using Assignment_A2_02.Services;

namespace Assignment_A2_02
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new NewsService();
            service.NewsAvailable += Service_NewsAvailable;

            Task<News>[] tasks = { null, null, null, null, null, null, null};

            try
            {
                tasks[0] = service.GetNewsAsync(NewsCategory.business);
                tasks[1] = service.GetNewsAsync(NewsCategory.entertainment);
                tasks[2] = service.GetNewsAsync(NewsCategory.general);
                tasks[3] = service.GetNewsAsync(NewsCategory.health);
                tasks[4] = service.GetNewsAsync(NewsCategory.science);
                tasks[5] = service.GetNewsAsync(NewsCategory.sports);
                tasks[6] = service.GetNewsAsync(NewsCategory.technology);

                Task.WaitAll(tasks);

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

                    Console.WriteLine();
                    Console.WriteLine($"News in Category {news.Category}");

                    foreach (var article in news.Articles)
                    {
                        Console.WriteLine($"    - {article.DateTime}: {article.Title}");
                    }
                }
            }
        }
    }
}
