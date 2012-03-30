namespace MVCAzureStore.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MVCAzureStore.Models;
    using Microsoft.ApplicationServer.Caching;

    public class ProductsRepository : IProductRepository
    {
        private static DataCacheFactory CacheFactory;
        private static DataCacheFactoryConfiguration FactoryConfig;
        private bool enableCache = false;
        private bool enableLocalCache = false;

        public ProductsRepository(bool enableCache, bool enableLocalCache)
        {
            this.enableCache = enableCache;
            this.enableLocalCache = enableLocalCache;

            if (enableCache)
            {
                if (enableLocalCache && (FactoryConfig == null || !FactoryConfig.LocalCacheProperties.IsEnabled))
                {
                    TimeSpan localTimeout = new TimeSpan(0, 0, 30);
                    DataCacheLocalCacheProperties localCacheConfig = new DataCacheLocalCacheProperties(10000, localTimeout, DataCacheLocalCacheInvalidationPolicy.TimeoutBased);
                    FactoryConfig = new DataCacheFactoryConfiguration();

                    FactoryConfig.LocalCacheProperties = localCacheConfig;
                    CacheFactory = new DataCacheFactory(FactoryConfig);
                }
                else if (!enableLocalCache && (FactoryConfig == null || FactoryConfig.LocalCacheProperties.IsEnabled))
                {
                    CacheFactory = null;
                }
            }

            if (CacheFactory == null)
            {
                FactoryConfig = new DataCacheFactoryConfiguration();
                CacheFactory = new DataCacheFactory(FactoryConfig);
            }
        }


        public List<string> GetProducts()
        {
            List<string> products = null;

            DataCache dataCache = null;
            if (enableCache)
            {
                try
                {
                    dataCache = CacheFactory.GetDefaultCache();
                    products = dataCache.Get("products") as List<string>;
                    if (products != null)
                    {
                        products[0] = "(from cache)";
                        return products;
                    }
                }
                catch (DataCacheException ex)
                {
                    if (ex.ErrorCode != DataCacheErrorCode.RetryLater)
                    {
                        throw;
                    }
                    // ignore temporary failures
                }
            }

            NorthwindEntities context = new NorthwindEntities();
            var query = from product in context.Products
                        select product.ProductName;
            products = query.ToList();
            products.Insert(0, "(from data source)");
            
            if (enableCache && dataCache != null)
            {
                dataCache.Add("products", products, TimeSpan.FromSeconds(30));
            }

            return products;
        }
    }
}