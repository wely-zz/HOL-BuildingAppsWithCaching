namespace CloudShop.Services
{
    using System.Collections.Generic;

    public interface IProductRepository
    {
        List<string> GetProducts();
    }
}
