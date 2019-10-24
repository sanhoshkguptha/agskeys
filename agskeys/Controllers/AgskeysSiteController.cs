using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace agskeys.Controllers
{
    public class AgskeysSiteController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: AgskeysSite
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(FormCollection form, vendor_table obj)
        //{
        //    if (form["userlevel"].ToString() == null || form["userlevel"].ToString() == "User type" || form["userlevel"].ToString() == "userlevel")
        //    {
        //        TempData["Message"] = "please select userrole";
        //        return RedirectToAction("Index", "AgskeysSiteController");
        //    }
        //    else if (form["userlevel"].ToString() == "partner")
        //    {
        //        string userName = form["userName"].ToString();
        //        string passwordfrom = form["password"].ToString();
        //        string userlevel = form["userlevel"].ToString();
        //        var vndr = (from u in ags.vendor_table where u.username == userName select u).FirstOrDefault();
        //        if (vndr == null)
        //        {
        //            //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
        //            TempData["Message"] = "username or password is wrong";
        //            return RedirectToAction("Index", "AgskeysSite");
        //        }
        //        else if (vndr != null)
        //        {
        //            var model = ags.vendor_table.Where(x => x.username == userName).SingleOrDefault();
        //            bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

        //            if (result)
        //            {
        //                Session["userid"] = vndr.id.ToString();
        //                Session["username"] = vndr.username.ToString();
        //                Session["userlevel"] = "partner";
        //                FormsAuthentication.SetAuthCookie(vndr.username, false);
        //                return RedirectToAction("Index", "Partner");
        //            }
        //            else
        //            {
        //                TempData["Message"] = "Enter the valid user credentials";
        //                return RedirectToAction("Index", "AgskeysSite");
        //            }
        //        }
        //        else
        //        {
        //            TempData["Message"] = "username or password is wrong";
        //            return RedirectToAction("Index", "AgskeysSite");
        //        }

        //    }
        //    else if (form["userlevel"].ToString() == "clientele")
        //    {
        //        string userName = form["userName"].ToString();
        //        string passwordfrom = form["password"].ToString();
        //        string userlevel = form["userlevel"].ToString();
        //        var customer = (from u in ags.customer_profile_table where u.customerid == userName select u).FirstOrDefault();
        //        if (customer == null)
        //        {
        //            //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
        //            TempData["Message"] = "username or password is wrong";
        //            return RedirectToAction("Index", "AgskeysSite");
        //        }
        //        else if (customer != null)
        //        {
        //            var model = ags.customer_profile_table.Where(x => x.customerid == userName).SingleOrDefault();
        //            bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

        //            if (result)
        //            {
        //                Session["userid"] = customer.id.ToString();
        //                Session["username"] = customer.customerid.ToString();
        //                Session["userlevel"] = "clientele";
        //                FormsAuthentication.SetAuthCookie(customer.customerid, false);
        //                return RedirectToAction("Index", "clientele");
        //            }
        //            else
        //            {
        //                TempData["Message"] = "Enter the valid user credentials";
        //                return RedirectToAction("Index", "AgskeysSite");
        //            }
        //        }
        //        else
        //        {
        //            TempData["Message"] = "username or password is wrong";
        //            return RedirectToAction("Index", "AgskeysSite");
        //        }

        //    }
        //    else if (form["userlevel"].ToString() == "sales_executive")
        //    {
        //        string userName = form["userName"].ToString();
        //        string passwordfrom = form["password"].ToString();
        //        string userlevel = form["userlevel"].ToString();
        //        var sales_executive = (from u in ags.admin_table where u.username == userName && u.isActive == true select u).FirstOrDefault();
        //        if (sales_executive == null)
        //        {
        //            //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
        //            TempData["Message"] = "username or password is wrong";
        //            return RedirectToAction("Index", "AgskeysSite");
        //        }
        //        else if (sales_executive != null)
        //        {
        //            var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
        //            bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);
        //            var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == userlevel && x.status == "publish").SingleOrDefault();


        //            if (result)
        //            {
        //                Session["userid"] = sales_executive.id.ToString();
        //                Session["username"] = sales_executive.username.ToString();
        //                FormsAuthentication.SetAuthCookie(sales_executive.username, false);
        //                if (emp.emp_category_id == "sales_executive" && emp.emp_category_id == model.userrole)
        //                {
        //                    Session["userlevel"] = form["userlevel"].ToString();
        //                    return RedirectToAction("Index", "SalesExecutive");
        //                }
        //                else
        //                {
        //                    TempData["Message"] = "Enter the valid user credentials";
        //                    return RedirectToAction("Index", "AgskeysSite");
        //                }
        //            }
        //            else
        //            {
        //                TempData["Message"] = "Enter the valid user credentials";
        //                return RedirectToAction("Index", "AgskeysSite");
        //            }
        //        }
        //        else
        //        {
        //            TempData["Message"] = "username or password is wrong";
        //            return RedirectToAction("Index", "AgskeysSite");
        //        }

        //    }
        //    return RedirectToAction("Index", "AgskeysSite");

        //}

     
        //[Authorize]
        //public ActionResult ClientLogout()
        //{
        //    FormsAuthentication.SignOut();
        //    Session.Abandon();
        //    return RedirectToAction("Index", "AgskeysSite");
        //}





        public ActionResult about()
        {
            return View();
        }
        public ActionResult business_loan()
        {
            return View();
        }
        public ActionResult contact_us()
        {
            return View();
        }
        public ActionResult emi()
        {
            return View();
        }
        public ActionResult home_loan()
        {
            return View();
        }
        public ActionResult lap()
        {
            return View();
        }
    }
}