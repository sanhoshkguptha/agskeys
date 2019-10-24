using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.Admin
{
    [Authorize]
    public class AdminController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: Admin
        
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin")
            {
                return this.RedirectToAction("Logout","Account");
            }
            var customerCount = ags.customer_profile_table.ToList().Count();
            var partnerCount = ags.vendor_table.ToList().Count();
            var employeeCount = ags.admin_table.Where(t => t.userrole != "super_admin").ToList().Count();
            var name = Session["username"].ToString();
            var photo = ags.admin_table.Where(t => t.userrole == "admin" && t.username == name).ToList();
            ViewData["photo"] = photo.FirstOrDefault().photo;
            ViewData["customerCount"] = customerCount.ToString();
            ViewData["partnerCount"] = partnerCount.ToString();
            ViewData["employeeCount"] = employeeCount.ToString();           
            return View("~/Views/Admin_Mangement/Admin/Index.cshtml");
        }
     
        public ActionResult Admin()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getEmployeeCategoty = ags.emp_category_table.ToList();
            var Admin = ags.admin_table.Where(x => x.userrole != "super_admin").OrderByDescending(t => t.id).ToList();
            var userrole = "";
            foreach (var item in Admin)
            {
                foreach (var items in getEmployeeCategoty)
                {
                    if (items.emp_category_id.ToString() == item.userrole)
                    {
                        userrole = items.emp_category + " (" + items.status + ")";
                        break;
                    }
                    else if (items.emp_category_id.ToString() != item.userrole)
                    {
                        userrole = "Not Updated";
                        continue;
                    }
                }
                item.userrole = userrole;

            }


            return View("~/Views/Admin_Mangement/Admin/Admin.cshtml",Admin);

        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getEmployeeCategoty = ags.emp_category_table.Where(x => x.status == "publish").ToList();
            SelectList list = new SelectList(getEmployeeCategoty, "emp_category_id", "emp_category");
            ViewBag.categoryList = list;
            var model = new agskeys.Models.admin_table();
            return PartialView("~/Views/Admin_Mangement/Admin/Create.cshtml",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(admin_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin" )
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                var usr = (from u in ags.admin_table where u.username == obj.username select u).FirstOrDefault();
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", ".jpeg"
                };

                if (usr == null)
                {
                    if (obj.ImageFile != null)
                    {
                        string BigfileName = Path.GetFileNameWithoutExtension(obj.ImageFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension = Path.GetExtension(obj.ImageFile.FileName);
                        if (allowedExtensions.Contains(extension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                            obj.photo = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            obj.ImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Admin");
                        }
                    }
                    obj.password = PasswordStorage.CreateHash(obj.password);
                    ags.admin_table.Add(new admin_table
                    {
                        name = obj.name,
                        email = obj.email,
                        phoneno = obj.phoneno,
                        alternatephone = obj.alternatephone,
                        dob = obj.dob,
                        userrole = obj.userrole,
                        username = obj.username,
                        password = obj.password,
                        photo = obj.photo,
                        isActive = obj.isActive,
                        address = obj.address,
                        datex = DateTime.Now.ToString(),
                        addedby = Session["username"].ToString()
                    });
                    ags.SaveChanges();
                    return RedirectToAction("Admin");
                }
                else
                {
                    TempData["AE"] = "This user name is already exist";
                    return View();
                    //return RedirectToAction("Admin");
                }
            }
            return View("~/Views/Admin_Mangement/Admin/Create.cshtml",obj);
        }

        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getEmployeeCategoty = ags.emp_category_table.ToList();
            var user = ags.admin_table.Where(x => x.id == Id).FirstOrDefault();
            foreach (var items in getEmployeeCategoty)
            {
                if (items.emp_category_id.ToString() == user.userrole)
                {
                    user.userrole = items.emp_category;

                }

            }


            return PartialView("~/Views/Admin_Mangement/Admin/Details.cshtml",user);
        }



        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var getEmployeeCategoty = ags.emp_category_table.Where(x => x.status == "publish").ToList();
            SelectList list = new SelectList(getEmployeeCategoty, "emp_category_id", "emp_category");
            ViewBag.categoryList = list;
            admin_table admin_table = ags.admin_table.Find(Id);

            if (admin_table == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/Admin_Mangement/Admin/Edit.cshtml",admin_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(admin_table admin_table)
        {
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", ".jpeg"
                };
                admin_table existing = ags.admin_table.Find(admin_table.id);
                var password = existing.password.ToString();
                var newPassword = admin_table.password.ToString();


                if (existing.photo == null && admin_table.ImageFile != null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(admin_table.ImageFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension = Path.GetExtension(admin_table.ImageFile.FileName);
                    if (allowedExtensions.Contains(extension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                        admin_table.photo = "~/adminimage/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                        admin_table.ImageFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                        return RedirectToAction("Admin");
                    }
                }


                else if (existing.photo != null && admin_table.photo != null)
                {
                    if (admin_table.ImageFile != null)
                    {
                        string path = Server.MapPath(existing.photo);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(admin_table.ImageFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension = Path.GetExtension(admin_table.ImageFile.FileName);
                        if (allowedExtensions.Contains(extension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                            admin_table.photo = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            admin_table.ImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Admin");
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
                existing.name = admin_table.name;
                existing.email = admin_table.email;
                existing.phoneno = admin_table.phoneno;
                existing.alternatephone = admin_table.alternatephone;
                existing.dob = admin_table.dob;
                existing.address = admin_table.address;
                existing.userrole = admin_table.userrole;
                if (existing.username != admin_table.username)
                {
                    var userCount = (from u in ags.admin_table where u.username == admin_table.username select u).Count();
                    if (userCount == 0)
                    {
                        existing.username = admin_table.username;
                    }
                    else
                    {
                        //existing.username = admin_table.username;
                        TempData["AE"] = "This user name is already exist";
                        //return PartialView("Edit", "Admin");
                        return RedirectToAction("Admin");
                    }
                }


                existing.isActive = admin_table.isActive;
                existing.photo = admin_table.photo;

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
                    existing.password = admin_table.password;
                }
                else
                {
                    existing.password = PasswordStorage.CreateHash(admin_table.password);
                }
                ags.SaveChanges();
                return RedirectToAction("Admin");
            }
            return PartialView("~/Views/Admin_Mangement/Admin/Edit.cshtml",admin_table);
        }

        // GET: vendor_table/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var getEmployeeCategoty = ags.emp_category_table.ToList();
            var user = ags.admin_table.Where(x => x.id == id).FirstOrDefault();
            foreach (var items in getEmployeeCategoty)
            {
                if (items.emp_category_id.ToString() == user.userrole)
                {
                    user.userrole = items.emp_category;

                }

            }

            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/Admin_Mangement/Admin/Delete.cshtml",user);
        }

        // POST: vendor_table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            admin_table admin_table = ags.admin_table.Find(id);
            string path = Server.MapPath(admin_table.photo);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }
            ags.admin_table.Remove(admin_table);
            ags.SaveChanges();
            return RedirectToAction("Admin");
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