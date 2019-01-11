using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sumazon.Models;

namespace Sumazon.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Shop()
        {
            return View();
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult Cart()
        {
            int i = Temp();
            return View();
        }

        public ActionResult AddToCart(int id)
        {
            int UserId = Convert.ToInt32(Session["userID"]);
            int count = 0;
            int quantity = 0;
            int ProductId = 0;
            int temp = id;
            while (temp != 0)
            {
                // n = n/10
                temp /= 10;
                ++count;
            }
            if (count == 5)
            {
                ProductId = id / 10;
                quantity = id % 10;
            }
            else if (count == 6)
            {
                ProductId = id / 100;
                quantity = id % 100;
            }
            else
            {
                ProductId = id / 1000;
                quantity = id % 1000;
            }
            //////////////////////////////////////////////////////////////////////
            try
            {
                if (ModelState.IsValid)
                {
                    using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew())
                    {
                        var CartDetails = Ece.Carts.Where(x => x.UId == UserId).FirstOrDefault();
                        if (CartDetails != null)
                        {
                            var ProductInDb = Ece.Carts.SingleOrDefault(p => p.PId == ProductId);
                            if (ProductInDb != null  && CartDetails.PId == ProductId)
                            {
                                ProductInDb.Quantity = ProductInDb.Quantity + quantity;
                                Ece.SaveChanges();
                                return View("Shop");
                            }
                            else
                            {
                                var newCart = Ece.Carts.Create();
                                newCart.UId = UserId;
                                newCart.PId = ProductId;
                                newCart.Quantity = quantity;
                                newCart.Status = false;
                                Ece.Carts.Add(newCart);
                                Ece.SaveChanges();
                            }
                        }
                        else
                        {
                            var newCart = Ece.Carts.Create();
                            newCart.UId = UserId;
                            newCart.PId = ProductId;
                            newCart.Quantity = quantity;
                            newCart.Status = false;
                            Ece.Carts.Add(newCart);
                            Ece.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return View("Shop");
            ///////////////////////////////////////////
        }

        public JsonResult ConvertInventory()
        {
            try
            {
                using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew())
                {
                    var myList = GetData(Ece);
                    return Json(myList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
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
                                   Price = temp.Product.Price,
                                   ProductName = temp.Product.Name,
                                   Quantity = temp.Quantity
                               }).OrderBy(temp => temp.Id)).Take(100).ToList();
                myList.RemoveAll(x => x.UserName != Session["userName"].ToString());
                return myList;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw new NotImplementedException();
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

        public int Temp()
        {
            ECommerceEntitiesNew ece = new ECommerceEntitiesNew();
            int GrandTotal = 0;
            var myList = ((from temp in ece.Carts
                           select new
                           {
                               Id = temp.Id,
                               Uid = temp.UId,
                               UserName = temp.User.Name,
                               Pid = temp.PId,
                               Price = temp.Product.Price,
                               ProductName = temp.Product.Name,
                               Quantity = temp.Quantity,
                               Status = temp.Status,
                               Total = temp.Quantity * temp.Product.Price
                           }).OrderBy(temp => temp.Id)).Take(100).ToList();
            myList.RemoveAll(x => x.UserName != Session["userName"].ToString());
            foreach (var item in myList)
            {
                GrandTotal = (int)(GrandTotal + item.Total);
            }
            ViewBag.GrandTot = GrandTotal;
            Session["Total"] = GrandTotal;
            var temp2 = Session["Total"];
            return GrandTotal;

        }

        public JsonResult ConvertCart()
        {
            try
            {
                using (ECommerceEntitiesNew Ece = new ECommerceEntitiesNew())
                {
                    var myList = GetCart(Ece);
                    return Json(myList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult PlaceOrder()
        {
            ECommerceEntitiesNew Ece = new ECommerceEntitiesNew();
            var CartElements = Ece.Carts;
            foreach(var Element in CartElements)
            {
                if (Element.UId == Convert.ToInt32(Session["userID"]))
                {
                    var newOrder = Ece.Orders.Create();
                    newOrder.UserId = Element.UId;
                    newOrder.ProductId = Element.PId;
                    newOrder.Quantity = Element.Quantity.ToString();
                    Ece.Orders.Add(newOrder);
                    Ece.Carts.Remove(Element);
                }

            }
            Ece.SaveChanges();
            Session["Total"] = 0;
            return View("Cart");
        }

        public object GetCart(ECommerceEntitiesNew ece)
        {
            int GrandTotal =0;
            try
            {
                var myList = ((from temp in ece.Carts
                               select new
                               {
                                   Id = temp.Id,
                                   Uid = temp.UId,
                                   UserName = temp.User.Name,
                                   Pid = temp.PId,
                                   Price = temp.Product.Price,
                                   ProductName = temp.Product.Name,
                                   Quantity = temp.Quantity,
                                   Status =temp.Status,
                                   Total = temp.Quantity * temp.Product.Price
                               }).OrderBy(temp => temp.Id)).Take(100).ToList();
                myList.RemoveAll(x => x.UserName != Session["userName"].ToString());
                foreach (var item in myList)
                {
                    GrandTotal = (int)(GrandTotal + item.Total);
                }
                ViewBag.GrandTot = GrandTotal;
                Session["Total"] = GrandTotal;
                var temp2 = Session["Total"];
                return myList;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw new NotImplementedException();
            }
        }
    }
}