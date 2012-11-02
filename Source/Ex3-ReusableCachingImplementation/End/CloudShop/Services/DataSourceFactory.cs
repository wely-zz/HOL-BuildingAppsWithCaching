using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Runtime.Caching;
using CloudShop.Services;
using CloudShop.Services.Caching;

namespace CloudShop.Services
{
    public class DataSourceFactory
    {
        private static readonly ObjectCache cacheProvider;

        static DataSourceFactory()
        {
            string provider = ConfigurationManager.AppSettings["CacheService.Provider"];
            if (provider != null)
            {
                switch (ConfigurationManager.AppSettings["CacheService.Provider"].ToUpperInvariant())
                {
                    case "AZURE":
                        cacheProvider = new AzureCacheProvider();
                        break;
                    case "INMEMORY":
                        cacheProvider = MemoryCache.Default;
                        break;
                }
            }
        }

        public static ObjectCache CacheProvider
        {
            get { return cacheProvider; }
        }

        public static IProductRepository GetProductsRepository(bool enableCache)
        {
            var dataSource = new ProductsRepository();
            if (enableCache && CacheProvider != null)
            {
                return new CachedProductsRepository(dataSource, cacheProvider);
            }

            return dataSource;
        }
    }
}