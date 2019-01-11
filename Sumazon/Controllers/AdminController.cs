using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sumazon.Models;

namespace Sumazon.Controllers
{
    public class AdminController : Controller
    {
        ECommerceEntitiesNew ece = new ECommerceEntitiesNew();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inventory()
        {
            return View();
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult New()
        {
            IEnumerable<SelectListItem> items = ece.Categories.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,

            });

            ViewBag.cid = items;
            return View();
        }

        public JsonResult ConvertData() 
        { 
            try 
            { 
                using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew()) 
                { 
                    var myList = GetData( Ece);                    
                    return Json(myList, JsonRequestBehavior.AllowGet); 
                } 
            } 
            catch (Exception) 
            { 
                throw; 
            } 
        }

        public object GetData(ECommerceEntitiesNew ece)
        {

            try
            {
                var myList = ((from temp in ece.Products
                               select new
                               {
                                   Id = temp.Id,
                                   Name = temp.Name,
                                   Price = temp.Price,
                                   Category = temp.Category,
                                   Discontinued = temp.Discontinued
                               }).OrderBy(temp => temp.Id)).Take(100).ToList();
                myList.RemoveAll(x => x.Discontinued == true);
                return myList;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw new NotImplementedException();
            }
        }

        public JsonResult ConvertOrders()
        {
            try
            {
                using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew())
                {
                    var myList = GetOrders(Ece);
                    return Json(myList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object GetOrders(ECommerceEntitiesNew ece)
        {

            try
            {
                var myList = ((from temp in ece.Orders
                               select new
                               {
                                   Id = temp.Id,
                                   UserName = temp.User.Name,
                                   ProductName = temp.Product.Name,
                                   Quantity = temp.Quantity
                               }).OrderBy(temp => temp.Id)).Take(100).ToList();
                //myList.RemoveAll(x => x.Discontinued == true);
                return myList;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw new NotImplementedException();
            }
        }

        //DELETE /api/customers/1
        public ActionResult DeleteProduct(int id)
        {
            ECommerceEntitiesNew Ece = new ECommerceEntitiesNew();
            var ProductInDb = Ece.Products.SingleOrDefault(p => p.Id == id);
            Console.Write("Inside delete function");
            if (ProductInDb == null)
                Console.Write("Product not found");
            //Ece.Products.Remove(ProductInDb);
            ProductInDb.Discontinued = true;
            Ece.SaveChanges();
            return View("Inventory");
        }

        /**/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product product, string cid)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew())
                    {
                        var productDetails = Ece.Users.Where(x => x.Name == product.Name).FirstOrDefault();
                        if (productDetails != null)
                        {
                            Console.Write("Product already Exists");
                            return View("New");
                        }
                        else
                        {
                            var newProduct = Ece.Products.Create();
                            newProduct.Name = product.Name;
                            newProduct.Price = product.Price;
                            newProduct.Category = cid;
                            newProduct.Discontinued = false;
                            Ece.Products.Add(newProduct);
                            Ece.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return RedirectToAction("Inventory", "Admin");
        }
        
    }
}