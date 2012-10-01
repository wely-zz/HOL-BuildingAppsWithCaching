using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ServiceRuntime;
using CloudShop.Models;

namespace CloudShop.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            return this.View();
        }

        public EmptyResult Recycle()
        {
            RoleEnvironment.RequestRecycle();
            return new EmptyResult();
        }

        public ActionResult Index()
        {
            Services.IProductRepository productRepository = new Services.ProductsRepository();
            var products = productRepository.GetProducts();

            // add all products currently not in session
            var itemsInSession = this.Session["Cart"] as List<string> ?? new List<string>();
            var filteredProducts = products.Where(item => !itemsInSession.Contains(item));

            IndexViewModel model = new IndexViewModel()
            {
                Products = filteredProducts
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
