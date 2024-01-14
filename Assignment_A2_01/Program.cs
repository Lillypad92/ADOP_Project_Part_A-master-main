using Assignment_A2_01.Models;
using Assignment_A2_01.Services;

namespace Assignment_A2_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new NewsService();
            
            try
            {
                var newsApiData = await service.GetNewsAsync();
                WriteNews(newsApiData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        private static void WriteNews(NewsApiData newsApiData)
        {
            Console.WriteLine("Top headlines:");

            foreach (var article in newsApiData.Articles)
            {
                Console.WriteLine(article.Title);
            }      
        }
    }
}
