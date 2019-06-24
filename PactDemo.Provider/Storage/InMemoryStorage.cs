using PactDemo.Provider.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PactDemo.Provider.Storage
{
    public class InMemoryStorage
    {
        private static readonly IDictionary<Guid, Product> products = new Dictionary<Guid, Product>();

        public async Task<Product> SaveProduct(Product product)
        {
            return await Task.Run(() =>
            {
                if (products.ContainsKey(product.Id))
                {
                    products[product.Id] = product;
                }
                else
                {
                    products.Add(product.Id, product);
                }
                return product;
            });
        }

        public async Task<Product> GetProduct(Guid id)
        {
            return await Task.FromResult(products[id]);
        }
    }
}
