using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.Admin
{
    [Authorize]
    public class VendorAdminController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult VendorAdmin()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var vendoradmin = (from vendor in ags.vendor_table orderby vendor.id descending select vendor).ToList();
            return View("~/Views/Admin_Mangement/VendorAdmin/VendorAdmin.cshtml", vendoradmin);
        }
        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var vendoradmin = ags.vendor_table.Where(x => x.id == Id).FirstOrDefault();
            return PartialView("~/Views/Admin_Mangement/VendorAdmin/Details.cshtml", vendoradmin);
        }
    }
}