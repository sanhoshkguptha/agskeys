using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Vendor()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var vendors = (from vendor in ags.vendor_table orderby vendor.id descending select vendor).ToList();

            return PartialView(vendors);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var model = new agskeys.Models.vendor_table();
            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(vendor_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                var vendor = (from u in ags.vendor_table where u.username == obj.username select u).FirstOrDefault();


                if (vendor == null)
                {
                    obj.password = PasswordStorage.CreateHash(obj.password);
                    ags.vendor_table.Add(new vendor_table
                    {
                        name = obj.name,
                        email = obj.email,
                        phoneno = obj.phoneno,
                        companyname = obj.companyname,
                        username = obj.username,
                        password = obj.password,
                        address = obj.address,
                        datex = DateTime.Now.ToString(),
                        addedby = Session["username"].ToString()
                    });
                    ags.SaveChanges();
                    return RedirectToAction("Vendor", "Vendor");
                }
                else
                {
                    TempData["AE"] = "This vendor user name is already exist";
                    return RedirectToAction("Vendor");
                }
            }
            return View(obj);
        }
        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var vendor = ags.vendor_table.Where(x => x.id == Id).FirstOrDefault();
            return PartialView(vendor);
        }
        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vendor_table vendor_table = ags.vendor_table.Find(Id);
            if (vendor_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(vendor_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(vendor_table vendor_table)
        {
            if (ModelState.IsValid)
            {
                vendor_table existing = ags.vendor_table.Find(vendor_table.id);
                var password = existing.password.ToString();
                var newPassword = vendor_table.password.ToString();
                existing.name = vendor_table.name;
                existing.email = vendor_table.email;
                existing.phoneno = vendor_table.phoneno;
                existing.companyname = vendor_table.companyname;
                existing.address = vendor_table.address;
                if (existing.username != vendor_table.username)
                {
                    var userCount = (from u in ags.vendor_table where u.username == vendor_table.username select u).Count();
                    if (userCount == 0)
                    {
                        existing.username = vendor_table.username;
                    }
                    else
                    {
                        TempData["AE"] = "This user name is already exist";
                        return RedirectToAction("Vendor");
                    }
                }

                if (existing.addedby == null)
                {
                    existing.addedby = Session["username"].ToString();
                }
                else
                {
                    existing.addedby = existing.addedby;
                }
                if (existing.datex == null)
                {
                    existing.datex = DateTime.Now.ToString();
                }
                else
                {
                    existing.datex = existing.datex;
                }
                if (password.Equals(newPassword))
                {
                    existing.password = vendor_table.password;
                }
                else
                {
                    existing.password = PasswordSecurity.PasswordStorage.CreateHash(vendor_table.password);
                }
                ags.SaveChanges();
                return RedirectToAction("Vendor", "Vendor");
            }
            return PartialView(vendor_table);
        }

        public ActionResult Delete(int? id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vendor_table vendor_table = ags.vendor_table.Find(id);
            if (vendor_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(vendor_table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            vendor_table vendor_table = ags.vendor_table.Find(id);
            ags.vendor_table.Remove(vendor_table);
            ags.SaveChanges();
            return RedirectToAction("Vendor", "Vendor");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ags.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}