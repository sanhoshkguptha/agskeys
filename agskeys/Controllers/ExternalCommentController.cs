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
    public class ExternalCommentController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult ExternalComment()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var extComment = (from sub in ags.external_comment_table orderby sub.id descending select sub).ToList();

            return View(extComment);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var model = new agskeys.Models.external_comment_table();
            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(external_comment_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {  
                ags.external_comment_table.Add(new external_comment_table
                {
                    externalcomment = obj.externalcomment,                    
                    datex = DateTime.Now.ToString(),
                    addedby = Session["username"].ToString()
                });
                ags.SaveChanges();
                return RedirectToAction("ExternalComment");
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
            external_comment_table external_comment_table = ags.external_comment_table.Find(Id);
            if (external_comment_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(external_comment_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(external_comment_table ExtComment)

        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {

                external_comment_table existing = ags.external_comment_table.Find(ExtComment.id);

                existing.externalcomment = ExtComment.externalcomment;

                if (existing.addedby == null)
                {
                    existing.addedby = Session["username"].ToString();
                }
                if (existing.datex == null)
                {
                    existing.datex = DateTime.Now.ToString();
                }

                ags.SaveChanges();
                return RedirectToAction("ExternalComment");
            }
            return PartialView(ExtComment);

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
            external_comment_table external_comment_table = ags.external_comment_table.Find(id);
            if (external_comment_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(external_comment_table);
        }
        // POST: vendor_table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            external_comment_table external_comment_table = ags.external_comment_table.Find(id);
            ags.external_comment_table.Remove(external_comment_table);
            ags.SaveChanges();
            return RedirectToAction("ExternalComment");
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