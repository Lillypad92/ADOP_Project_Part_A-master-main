#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using Assignment_A2_04.Models;
using Assignment_A2_04.ModelsSampleData;
using System.Net;

namespace Assignment_A2_04.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();

        // Your API Key
        readonly string apiKey = "1c2ed5ab06ce461d91e907f429d953aa";

        //Event declaration
        public event EventHandler<string> NewsAvailable;

        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public async Task<News> GetNewsAsync(NewsCategory category)
        {
#if UseNewsApiSample      
            
            //Cache
            var cache = new NewsCacheKey(category, DateTime.Now);
            if (cache.CacheExist)
            {
                NewsAvailable.Invoke(this, $"Event message from news service: XML Cached news in category is available: {category}");
                return News.Deserialize(cache.FileName);
            }

            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category.ToString());

#else
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}";

            // make the http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            //Convert Json to Object
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();
#endif
            
            var news = new News()
            {
                Category = category,
                Articles = nd.Articles.Select(x => new NewsItem()
                {
                    DateTime = x.PublishedAt,
                    Title = x.Title
                }).ToList()
            };

            //SERIALIZE IF NOT CACHED
            News.Serialize(news, cache.FileName);

            NewsAvailable.Invoke(this, $"Event message from news service: News in category is available: {category}");

            return news;
        }
    }

}
