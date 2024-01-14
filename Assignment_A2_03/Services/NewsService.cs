#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Assignment_A2_03.Models;
using Assignment_A2_03.ModelsSampleData;
using System.Net;

namespace Assignment_A2_03.Services
{
    public class NewsService
    {
        //Event declaration
        public event EventHandler<string> NewsAvailable;

        //cache declaration
        ConcurrentDictionary<(string, string), News> cachedCategoryNews = new ConcurrentDictionary<(string, string), News>();

        HttpClient httpClient = new HttpClient();

        // Your API Key
        readonly string apiKey = "1c2ed5ab06ce461d91e907f429d953aa";

        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public async Task<News> GetNewsAsync(NewsCategory category)
        {
            //Check if news for category is cached, then return those news
            if (cachedCategoryNews.ContainsKey((category.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm"))))
            {
                NewsAvailable.Invoke(this, $"Event message from news service: Cached news in category is available: {category}");

                return cachedCategoryNews[(category.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm"))];
            }


#if UseNewsApiSample      
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

            var news = new News
            {
                Category = category,
                Articles = nd.Articles.Select(x => new NewsItem()
                {
                    DateTime = x.PublishedAt,
                    Title = x.Title
                }).ToList()
            };

            if (news.Articles.Count > 0)
            {
                cachedCategoryNews.AddOrUpdate((category.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm")), news, (key, existingNews) => existingNews);

                NewsAvailable.Invoke(this, $"Event message from news service: News in Category is available: {category}");
            }
            else if (news.Articles.Count == 0)
            {
                NewsAvailable.Invoke(this, $"Event message from news service: There is no News in category: {category}");
            }

            return news;
        }
    }
}
