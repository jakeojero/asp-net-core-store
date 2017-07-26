using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Casestudy.Models;
using Casestudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Casestudy.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Casestudy.Controllers
{
    public class RegisterController : Controller
    {
        UserManager<ApplicationUser> _usrMgr;
        SignInManager<ApplicationUser> _signInMgr;

        public RegisterController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _usrMgr = userManager;
            _signInMgr = signInManager;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST:/Register/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create new AppUser and set fields to POSTed model
                var user = new ApplicationUser { UserName = model.Email,
                                                 Email = model.Email,
                                                 Age = model.Age,
                                                 Address1 = model.Address1,
                                                 CreditcardType = model.CreditcardType,
                                                 Region = model.Region,
                                                 City = model.City,
                                                 Country = model.Country,
                                                 Firstname = model.Firstname,
                                                 Lastname = model.Lastname,
                                                 Mailcode = model.Mailcode
                                               };

                // Create the user
                var result = await _usrMgr.CreateAsync(user, model.Password);

                ViewBag.RegisterError = false;
                if (result.Succeeded)
                {
                    // Sign User in
                    await _signInMgr.SignInAsync(user, isPersistent: false);
                    HttpContext.Session.SetString(SessionVars.User, user.Email);
                    HttpContext.Session.SetString(SessionVars.LoginStatus, "logged on as " + model.Email);
                    HttpContext.Session.SetString(SessionVars.Message, "Registered, logged on as " + model.Email);
                }
                else
                {
                    ViewBag.RegisterError = true;
                    ViewBag.message = "Registration failed - " + result.Errors.First().Description;
                    return View("Index");
                }
            }
            return Redirect("/Home");
        }
    }
}