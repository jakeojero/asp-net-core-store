using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Casestudy.Models;
using Casestudy.ViewModels;

namespace Casestudy.Controllers
{
    public class ProductController : Controller
    {

        private AppDbContext _db;
        public ProductController(AppDbContext context)
        {
            _db = context;
        }

        public IActionResult Index(BrandViewModel brand)
        {
            ProductModel model = new ProductModel(_db);
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.BrandName = brand.BrandName;
            viewModel.Products = model.GetAllByBrandName(brand.BrandName);
            return View(viewModel);
        }
    }
}