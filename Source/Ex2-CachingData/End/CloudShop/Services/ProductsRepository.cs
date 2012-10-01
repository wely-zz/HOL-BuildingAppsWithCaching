namespace CloudShop.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CloudShop.Models;
    using Microsoft.ApplicationServer.Caching;

    public class ProductsRepository : IProductRepository
    {
        private static DataCacheFactory cacheFactory;
        private static DataCacheFactoryConfiguration factoryConfig;
        private bool enableCache = false;
        private bool enableLocalCache = false;

        public ProductsRepository(bool enableCache, bool enableLocalCache)
        {
            this.enableCache = enableCache;
            this.enableLocalCache = enableLocalCache;

            if (enableCache)
            {
                if (enableLocalCache && (factoryConfig == null || !factoryConfig.LocalCacheProperties.IsEnabled))
                {
                    TimeSpan localTimeout = new TimeSpan(0, 0, 30);
                    DataCacheLocalCacheProperties localCacheConfig = new DataCacheLocalCacheProperties(10000, localTimeout, DataCacheLocalCacheInvalidationPolicy.TimeoutBased);
                    factoryConfig = new DataCacheFactoryConfiguration();

                    factoryConfig.LocalCacheProperties = localCacheConfig;
                    cacheFactory = new DataCacheFactory(factoryConfig);
                }
                else if (!enableLocalCache && (factoryConfig == null || factoryConfig.LocalCacheProperties.IsEnabled))
                {
                    cacheFactory = null;
                }
            }

            if (cacheFactory == null)
            {
                factoryConfig = new DataCacheFactoryConfiguration();
                cacheFactory = new DataCacheFactory(factoryConfig);
            }
        }

        public List<string> GetProducts()
        {
            List<string> products = null;

            DataCache dataCache = null;
            if (this.enableCache)
            {
                try
                {
                    dataCache = cacheFactory.GetDefaultCache();
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

            products.Insert(0, "(from data source)");

            if (this.enableCache && dataCache != null)
            {
                dataCache.Add("products", products, TimeSpan.FromSeconds(30));
            }

            return products;
        }
    }
}