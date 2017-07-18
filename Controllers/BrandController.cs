using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Casestudy.Models;
using Casestudy.ViewModels;
using Casestudy.Utils;
using Newtonsoft.Json;

namespace Casestudy.Controllers
{
    public class BrandController : Controller
    {

        private AppDbContext _db;

        public BrandController(AppDbContext context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            BrandViewModel vm = new BrandViewModel();
            // only build the catalogue once
            if (HttpContext.Session.Get<List<Brand>>(SessionVars.Brands) == null)
            {
                try
                {
                    BrandModel brandModel = new BrandModel(_db);
                    // now load the categories
                    List<Brand> brands = brandModel.GetAll();
                    HttpContext.Session.Set<List<Brand>>(SessionVars.Brands, brands);
                    vm.SetBrands(brands);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Catalogue Problem - " + ex.Message;
                }
            }
            else
            {
                vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVars.Brands));
            }
            return View(vm);
        }

        public IActionResult SelectBrand (BrandViewModel vm)
        {
            BrandModel brandModel = new BrandModel(_db);
            ProductModel productModel = new ProductModel(_db);
            List<Product> products = productModel.GetAllByBrand(vm.BrandId);
            List<ProductViewModel> vms = new List<ProductViewModel>();

            if (products.Count > 0)
            {
                foreach (Product item in products)
                {
                    ProductViewModel pvm = new ProductViewModel();
                    pvm.BrandId = item.BrandId;
                    pvm.BrandName = brandModel.GetName(item.BrandId);
                    pvm.Description = item.Description;
                    pvm.Id = item.Id;
                    pvm.ProductName = item.ProductName;
                    pvm.GraphicName = item.GraphicName;
                    pvm.Qty = 0;
                    pvm.QtyOnBackOrder = item.QtyOnBackOrder;
                    pvm.QtyOnHand = item.QtyOnHand;
                    pvm.MSRP = item.MSRP;
                    pvm.CostPrice = item.CostPrice;

                    vms.Add(pvm);
                }

                ProductViewModel[] myProducts = vms.ToArray();
                HttpContext.Session.Set<ProductViewModel[]>(SessionVars.Products, myProducts);
            }

            vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVars.Brands));
            return View("Index", vm);
        }

        [HttpPost]
        public ActionResult SelectItem(BrandViewModel vm)
        {
            Dictionary<string, object> cart;
            if (HttpContext.Session.Get<Dictionary<string, object>>(SessionVars.Cart) == null)
            {
                cart = new Dictionary<string, object>();
            }
            else
            {
                cart = HttpContext.Session.Get<Dictionary<string, object>>(SessionVars.Cart);
            }
            ProductViewModel[] menu = HttpContext.Session.Get<ProductViewModel[]>(SessionVars.Products);
            String retMsg = "";
            foreach (ProductViewModel item in menu)
            {
                if (item.Id == vm.Id)
                {
                    var id = item.Id;
                    if (vm.Qty > 0) // update only selected item
                    {
                        // If Item exists in the cart
                        if (cart.ContainsKey(id))
                        {
                            var existingItem = JsonConvert.DeserializeObject<ProductViewModel>(cart[id].ToString());
                            existingItem.Qty += vm.Qty;
                            cart[id] = existingItem;
                        }
                        else
                        {
                            item.Qty = vm.Qty;
                            cart[id] = item;
                        }

                        retMsg = vm.Qty + " - item(s) Added!";
                    }
                    else
                    {
                        item.Qty = 0;
                        cart.Remove(id);
                        retMsg = "item(s) Removed!";
                    }
                    vm.BrandId = item.BrandId;
                    break;
                }
            }

            ViewBag.showCartButton = cart.Count > 0;

            ViewBag.AddMessage = retMsg;
            HttpContext.Session.Set<ProductViewModel[]>(SessionVars.Products, menu);
            HttpContext.Session.Set<Dictionary<string, object>>(SessionVars.Cart, cart);
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVars.Brands));
            return View("Index", vm);

        }
    }
}