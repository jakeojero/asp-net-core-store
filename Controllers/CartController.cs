using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casestudy.Models;
using Casestudy.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Casestudy.Controllers
{
    public class CartController : Controller
    {
        private AppDbContext _db;

        public CartController(AppDbContext context)
        {
            _db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult AddOrder()
        {
            // Must be logged in
            if (HttpContext.Session.GetString(SessionVars.User) == null)
            {
                return Redirect("/Login");
            }

            OrderModel model = new OrderModel(_db);
            int retVal = -1;
            string retMessage = "";
            string addedMessage = "";
            try
            {
                Dictionary<string, object> orderItems =
                    HttpContext.Session.Get<Dictionary<string, object>>(SessionVars.Cart);
                retVal = model.AddOrder(orderItems, HttpContext.Session.GetString(SessionVars.User), ref addedMessage);
                if (retVal > 0) //Order Added
                {
                    retMessage = "Order " + retVal + " Created! " + addedMessage;
                }
                else
                {
                    retMessage = "Tray not added, try again later";
                }
            }
            catch (Exception ex)
            {
                retMessage = "Order was not created, try again later! - " + ex.Message;
            }
            HttpContext.Session.Remove(SessionVars.Cart);
            HttpContext.Session.SetString(SessionVars.Message, retMessage);
            return Redirect("/Home");
        }

        public ActionResult ClearCart()
        {
            HttpContext.Session.Remove("cart");
            return Redirect("/Home");
        }
    }
}