using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PactDemo.Consumer
{
    public class ProductClient
    {
        private readonly HttpClient httpClient;

        public ProductClient(string productUri )
        {
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(productUri)
            };
        }

        public async Task<Product> Get(Guid id)
        {
            var response = await httpClient.GetAsync($"{id}");
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Content: " + content);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Product>(content);
        }
    }
}
