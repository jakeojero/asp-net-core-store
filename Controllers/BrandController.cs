using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Casestudy.Models;
using Casestudy.ViewModels;
using Casestudy.Utils;

namespace Casestudy.Controllers
{
    public class BrandController : Controller
    {

        AppDbContext _db;

        public BrandController(AppDbContext context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            BrandViewModel vm = new BrandViewModel();
            // only build the catalogue once
            if (HttpContext.Session.Get<List<Brand>>("categories") == null)
            {
                try
                {
                    BrandModel brandModel = new BrandModel(_db);
                    // now load the categories
                    List<Brand> brands = brandModel.GetAll();
                    HttpContext.Session.Set<List<Brand>>("brands", brands);
                    vm.SetBrands(brands);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Catalogue Problem - " + ex.Message;
                }
            }
            else
            {
                vm.SetBrands(HttpContext.Session.Get<List<Brand>>("brands"));
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
                HttpContext.Session.Set<ProductViewModel[]>("catalog", myProducts);
            }

            vm.SetBrands(HttpContext.Session.Get<List<Brand>>("brands"));
            return View("Index", vm);
        }

        [HttpPost]
        public ActionResult SelectItem(BrandViewModel vm)
        {
            Dictionary<int, object> tray;
            if (HttpContext.Session.Get<Dictionary<int, Object>>("tray") == null)
            {
                tray = new Dictionary<int, object>();
            }
            else
            {
                tray = HttpContext.Session.Get<Dictionary<int, object>>("tray");
            }
            ProductViewModel[] menu = HttpContext.Session.Get<ProductViewModel[]>("catalog");
            String retMsg = "";
            foreach (ProductViewModel item in menu)
            {
                if (Convert.ToInt32(item.Id) == vm.Id)
                {
                    var id = Convert.ToInt32(item.Id);
                    if (vm.Qty > 0) // update only selected item
                    {
                        item.Qty = vm.Qty;
                        retMsg = vm.Qty + " - item(s) Added!";
                        tray[id] = item;
                    }
                    else
                    {
                        item.Qty = 0;
                        tray.Remove(id);
                        retMsg = "item(s) Removed!";
                    }
                    vm.BrandId = item.BrandId;
                    break;
                }
            }
            ViewBag.AddMessage = retMsg;
            HttpContext.Session.Set<Dictionary<int, Object>>("tray", tray);
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>("brands"));
            return View("Index", vm);

        }
    }
}