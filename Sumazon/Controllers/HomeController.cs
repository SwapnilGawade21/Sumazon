using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sumazon.Models;
namespace Sumazon.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {   
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyUser(User user)
        {
            if (ModelState.IsValid)
            {
                using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew())
                {
                    var userDetails = Ece.Users.Where(x => x.Name  == user.Name && x.Password == user.Password).FirstOrDefault();
                    if (userDetails == null)
                    {
                        Console.Write("failed");
                        return View("Index");
                    }
                    else
                    {
                        if (user.Name == "Admin")
                        {
                            Session["userID"] = userDetails.Id;
                            Session["userName"] = userDetails.Name;
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            Session["userID"] = userDetails.Id;
                            Session["userName"] = userDetails.Name;
                            return RedirectToAction("Index", "Customer");
                        }
                    }
                }
            }
            ModelState.Clear();
            return RedirectToAction("Index","Home");
        }

        public ActionResult LogOut()
        {

            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew())
                    {
                        var userDetails = Ece.Users.Where(x => x.Name == user.Name).FirstOrDefault();
                        if (userDetails != null)
                        {
                            Console.Write("Username already Exists");
                            return View("Register");
                        }
                        else
                        {
                            var newUser = Ece.Users.Create();
                            newUser.Name = user.Name;
                            newUser.Password = user.Password;
                            newUser.RoleId = 2;
                            Ece.Users.Add(newUser);
                            Ece.SaveChanges();
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            ModelState.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}