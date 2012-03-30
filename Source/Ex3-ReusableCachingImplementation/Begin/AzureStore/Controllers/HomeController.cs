namespace MVCAzureStore.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Diagnostics;
    using MVCAzureStore.Models;

    [HandleError]    
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            return View();
        }
                       
        public ActionResult Index()
        {
            bool enableCache = (bool)this.Session["EnableCache"];

            // retrieve product catalog from repository and measure the elapsed time
            Services.IProductRepository productRepository =
                new Services.ProductsRepository();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var products = productRepository.GetProducts();
            stopWatch.Stop();

            // add all products currently not in session
            var itemsInSession = this.Session["Cart"] as List<string> ?? new List<string>();
            var filteredProducts = products.Where(item => !itemsInSession.Contains(item));

            IndexViewModel model = new IndexViewModel()
            {
                Products = filteredProducts,
                ElapsedTime = stopWatch.ElapsedMilliseconds,
                IsCacheEnabled = enableCache,
                ObjectId = products.GetHashCode().ToString()
            };

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add(string selectedItem)
        {
            if (selectedItem != null)
            {
                List<string> cart = this.Session["Cart"] as List<string> ?? new List<string>();
                cart.Add(selectedItem);
                Session["Cart"] = cart;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            var itemsInSession = this.Session["Cart"] as List<string> ?? new List<string>();
            return View(itemsInSession);
        }

        public ActionResult EnableCache(bool enabled)
        {
            this.Session["EnableCache"] = !((bool)this.Session["EnableCache"]);
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Remove(string selectedItem)
        {
            if (selectedItem != null)
            {
                var itemsInSession = this.Session["Cart"] as List<string>;
                if (itemsInSession != null)
                {
                    itemsInSession.Remove(selectedItem);
                }
            }

            return RedirectToAction("Checkout");
        }       
    }
}
