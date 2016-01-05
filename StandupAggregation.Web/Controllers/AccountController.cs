using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HipchatApiV2.Responses;
using StandupAggragation.Core.Services;
using StandupAggregation.Web.Models;

namespace StandupAggregation.Web.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View("Login");
        }


        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Form is not valid; please review and try again.";
                return View("Login");
            }

            var user = HipChatLogin(login);
            if (user != null)
            {
                FormsAuthentication.RedirectFromLoginPage(user.Id.ToString(), true);
            }
                
            
            ViewBag.Error = "Credentials invalid. Please try again.";
            return View("Login");
        }


        private HipchatUser HipChatLogin(LoginViewModel login)
        {
            var service = new HipChatLoginService();
            try
            {
                var result = service.Login(login.Username, login.Password);
                return result;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}