using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PactDemo.Provider.Model;
using PactDemo.Provider.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PactDemo.Provider.Tests
{
    public class ProviderStateMiddleware : IMiddleware
    {
        private const string ConsumerName = "PactDemo.Consumer";
        private readonly InMemoryStorage storage;
        private readonly IDictionary<string, Action> _providerStates;

        public ProviderStateMiddleware(InMemoryStorage storage)
        {
            this.storage = storage;
            _providerStates = new Dictionary<string, Action>
            {
                { "A Product with expected structure", () => {
                    this.storage.SaveProduct(new Product{
                        Id = Guid.Parse("e0c2e684-d83f-45c3-a6e5-d62a6f83a0bd"),
                        Name = "A test product",
                        Description = "A description for the test product"
                    }).GetAwaiter().GetResult();
                } }
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value == "/provider-states")
            {
                HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(string.Empty);
            }
            else
            {
                await next(context);
            }
        }

        private void HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() &&
                context.Request.Body != null)
            {
                string jsonRequestBody = string.Empty;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = reader.ReadToEnd();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null && !string.IsNullOrEmpty(providerState.State) &&
                    providerState.Consumer == ConsumerName)
                {
                    _providerStates[providerState.State].Invoke();
                }
            }
        }
    }
}