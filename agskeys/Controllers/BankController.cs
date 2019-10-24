using agskeys.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    [Authorize]
    public class BankController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Bank()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var banks = (from bank in ags.bank_table orderby bank.id descending select bank).ToList();

            return View(banks);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var model = new agskeys.Models.bank_table();
            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(bank_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                var bank = (from u in ags.bank_table where u.bankname == obj.bankname select u).FirstOrDefault();
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", ".jpeg"
                };

                if (bank == null)
                {
                    if (obj.ImageFile != null)
                    {
                        string BigfileName = Path.GetFileNameWithoutExtension(obj.ImageFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension1 = Path.GetExtension(obj.ImageFile.FileName);
                        string extension = extension1.ToLower();
                        if (allowedExtensions.Contains(extension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                            obj.photo = "~/bankImage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/bankImage/"), fileName);
                            obj.ImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Bank");
                        }
                    }
                   
                    ags.bank_table.Add(new bank_table
                    {
                        bankname = obj.bankname,
                        photo = obj.photo,
                        datex = DateTime.Now.ToString(),
                        addedby = Session["username"].ToString()
                    });
                    ags.SaveChanges();
                    return RedirectToAction("Bank", "Bank");
                }
                else
                {
                    TempData["AE"] = "This bank name is already exist";
                    return RedirectToAction("Bank");
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
            bank_table bank_table = ags.bank_table.Find(Id);
            if (bank_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(bank_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(bank_table bank_table)
        {
            if (ModelState.IsValid)
            {
                bank_table existing = ags.bank_table.Find(bank_table.id);
                var allowedExtensions = new[] {
                   ".Jpg", ".png", ".jpg", ".jpeg"
                };


                if (existing.photo == null && bank_table.ImageFile != null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(bank_table.ImageFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension1 = Path.GetExtension(bank_table.ImageFile.FileName);
                    string extension = extension1.ToLower();
                    if (allowedExtensions.Contains(extension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                        bank_table.photo = "~/bankImage/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/bankImage/"), fileName);
                        bank_table.ImageFile.SaveAs(fileName);
                        existing.photo = bank_table.photo;
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                        return RedirectToAction("Bank");
                    }
                }


                else if (existing.photo != null && bank_table.photo != null)
                {
                    if (bank_table.ImageFile != null)
                    {
                        string path = Server.MapPath(existing.photo);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(bank_table.ImageFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension1 = Path.GetExtension(bank_table.ImageFile.FileName);
                        string extension = extension1.ToLower();
                        if (allowedExtensions.Contains(extension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                            bank_table.photo = "~/bankImage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/bankImage/"), fileName);
                            bank_table.ImageFile.SaveAs(fileName);
                            existing.photo = bank_table.photo;
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Bank");
                        }

                    }
                    else
                    {
                        existing.photo = existing.photo;
                    }
                }
                else
                {
                    existing.photo = existing.photo;
                }

                if (existing.bankname != bank_table.bankname)
                {
                    var count = (from u in ags.bank_table where u.bankname == bank_table.bankname select u).Count();
                    if (count == 0)
                    {
                        existing.bankname = bank_table.bankname;
                    }
                    else
                    {
                        TempData["AE"] = "This bank name is already exist";
                        return RedirectToAction("Bank");
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
                return RedirectToAction("Bank");
            }
            return PartialView(bank_table);
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
            bank_table bank_table = ags.bank_table.Find(id);
            if (bank_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(bank_table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bank_table bank_table = ags.bank_table.Find(id);
            string path = Server.MapPath(bank_table.photo);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }
            ags.bank_table.Remove(bank_table);
            ags.SaveChanges();
            return RedirectToAction("Bank", "Bank");
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