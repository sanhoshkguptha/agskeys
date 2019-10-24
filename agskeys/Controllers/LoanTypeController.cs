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
    public class LoanTypeController : Controller
    {
        // GET: LoanType
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult loantype()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var loantype = (from sub in ags.loantype_table orderby sub.id descending select sub).ToList();

            return View(loantype);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var model = new agskeys.Models.loantype_table();
            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(loantype_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                var loantype = (from u in ags.loantype_table where u.loan_type == obj.loan_type select u).FirstOrDefault();

                if (loantype == null)
                {
                    ags.loantype_table.Add(new loantype_table
                    {
                        loan_type= obj.loan_type,
                        datex = DateTime.Now.ToString(),
                        addedby = Session["username"].ToString()
                    });
                    ags.SaveChanges();
                    return RedirectToAction("loantype");
                }
                else
                {
                    TempData["AE"] = "This Loan Type is already exist";
                    return RedirectToAction("loantype");
                }
            }
            return View(obj);
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
            loantype_table loantype_table = ags.loantype_table.Find(Id);
            if (loantype_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(loantype_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(loantype_table loantype_table)
        {
            if (ModelState.IsValid)
            {
                loantype_table existing = ags.loantype_table.Find(loantype_table.id);
                if (existing.loan_type != loantype_table.loan_type)
                {
                    var count = (from u in ags.loantype_table where u.loan_type == loantype_table.loan_type select u).Count();
                    if (count == 0)
                    {
                        existing.loan_type = loantype_table.loan_type;
                    }
                    else
                    {
                        TempData["AE"] = "This Loan Type is already exist";
                        return RedirectToAction("loantype");
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
                ags.SaveChanges();
                return RedirectToAction("loantype");
            }
            return PartialView(loantype_table);
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
            loantype_table loantype_table = ags.loantype_table.Find(id);
            if (loantype_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(loantype_table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            loantype_table loantype_table = ags.loantype_table.Find(id);
            ags.loantype_table.Remove(loantype_table);
            ags.SaveChanges();
            return RedirectToAction("loantype");
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