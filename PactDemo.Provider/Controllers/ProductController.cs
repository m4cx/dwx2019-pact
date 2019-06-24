using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PactDemo.Provider.Model;
using PactDemo.Provider.Storage;

namespace PactDemo.Provider.Controllers
{
    [Route("")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly InMemoryStorage storage;

        public ProductController(InMemoryStorage storage)
        {
            this.storage = storage;
        }

        [Route("{id}")]
        public async Task<Product> GetProduct(Guid id)
        {
            return await storage.GetProduct(id);
        }
    }
}