using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    [Authorize]
    public class EmpCategoryController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();

        //public ActionResult Index()
        //{
        //    if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
        //    {
        //        return this.RedirectToAction("Logout", "Account");
        //    }
        //    var empcategory = (from sub in ags.emp_category_table orderby sub.id descending select sub).ToList();

        //    return View(empcategory);
        //}
        public ActionResult Emp()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var empcategory = (from sub in ags.emp_category_table orderby sub.id descending select sub).ToList();

            return View(empcategory);
        }
        //[HttpGet]
        //public ActionResult Create()
        //{
        //    if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
        //    {
        //        return this.RedirectToAction("Logout", "Account");
        //    }
        //    var model = new agskeys.Models.emp_category_table();//load data from database by RestaurantId
        //    return PartialView(model);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(emp_category_table obj)
        //{
        //    if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
        //    {
        //        return this.RedirectToAction("Logout", "Account");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var usr = (from u in ags.emp_category_table where u.emp_category == obj.emp_category select u).FirstOrDefault();

        //        if (usr == null)
        //        {
        //            ags.emp_category_table.Add(new emp_category_table
        //            {
        //                emp_category = obj.emp_category,
        //                status = obj.status,
        //                datex = DateTime.Now.ToString(),
        //                addedby = Session["username"].ToString()
        //            });
        //            ags.SaveChanges();
        //            return RedirectToAction("Emp");
        //        }
        //        else
        //        {
        //            TempData["AE"] = "This Employee Category is already exist";
        //        }
        //    }
        //    return View(obj);
        //}
    

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
            emp_category_table emp_category_table = ags.emp_category_table.Find(Id);
            if (emp_category_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(emp_category_table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(emp_category_table empCategory)

        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {

                emp_category_table existing = ags.emp_category_table.Find(empCategory.id);

                if (empCategory.status == "publish")
                {
                    existing.status = empCategory.status;
                }
                else
                {
                    existing.status = empCategory.status;
                }

                //existing.emp_category = emp_category_table.emp_category;

                if (existing.emp_category != empCategory.emp_category)
                {
                    var userCount = (from u in ags.emp_category_table where u.emp_category == empCategory.emp_category select u).Count();
                    if (userCount == 0)
                    {
                        existing.emp_category = empCategory.emp_category;
                    }
                    else
                    {
                        TempData["AE"] = "This Employee Category is already exist";
                        return RedirectToAction("Edit", "EmpCategory");
                    }
                }

                if (existing.addedby == null)
                {
                    existing.addedby = Session["username"].ToString();
                }
                if (existing.datex == null)
                {
                    existing.datex = DateTime.Now.ToString();
                }

                ags.SaveChanges();
                return RedirectToAction("Emp", "EmpCategory");
            }
            return PartialView(empCategory);

        }

        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    emp_category_table emp_category_table = ags.emp_category_table.Find(id);
        //    if (emp_category_table == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return PartialView(emp_category_table);
        //}



        // POST: vendor_table/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    emp_category_table emp_category_table = ags.emp_category_table.Find(id);


        //    ags.emp_category_table.Remove(emp_category_table);
        //    ags.SaveChanges();
        //    return RedirectToAction("Emp");
        //}

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