using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CloudShop.Services;
using System.Runtime.Caching;

namespace CloudShop.Services.Caching
{
    public class CachedProductsRepository : CachedDataSource, IProductRepository
    {
        private readonly IProductRepository repository;

        public CachedProductsRepository(IProductRepository repository, ObjectCache cacheProvider) :
            base(cacheProvider, "Products")
        {
            this.repository = repository;
        }

        public List<string> GetProducts()
        {
            return RetrieveCachedData(
            "allproducts",
            () => this.repository.GetProducts(),
            new CacheItemPolicy { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(1) });
        }
    }
}