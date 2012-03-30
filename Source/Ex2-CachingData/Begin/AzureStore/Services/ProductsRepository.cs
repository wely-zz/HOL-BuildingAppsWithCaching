namespace MVCAzureStore.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MVCAzureStore.Models;

    public class ProductsRepository : IProductRepository
    {
        public List<string> GetProducts()
        {
            List<string> products = null;

            NorthwindEntities context = new NorthwindEntities();
            var query = from product in context.Products
                        select product.ProductName;
            products = query.ToList();

            return products;
        }
    }
}