namespace CloudShop.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CloudShop.Models;

    public class ProductsRepository : IProductRepository
    {
        public List<string> GetProducts()
        {
            List<string> products = null;

            NorthwindEntities context = new NorthwindEntities();

            try
            {
                var query = from product in context.Products
                            select product.ProductName;
                products = query.ToList();
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }

            return products;
        }
    }
}