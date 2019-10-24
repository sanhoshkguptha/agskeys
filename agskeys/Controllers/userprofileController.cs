using agskeys.Models;
using PasswordSecurity;
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
    public class userprofileController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult UserProfile()
        {
            if (Session["username"] == null && Session["userid"] == null)
            {
                return this.RedirectToAction("Logout", "Account");
            }
            else
            {
                var intId = Session["userid"].ToString();
                var Id = Convert.ToInt32(intId);
                var user = ags.admin_table.Where(x => x.id == Id).FirstOrDefault();
                return PartialView(user);
            }           

        }
        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null && Session["userid"] == null)
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
            return PartialView(admin_table);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(admin_table admin_table,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", ".jpeg"
                };
                admin_table existing = ags.admin_table.Find(admin_table.id);
                var password = existing.password.ToString();
                var newPassword = admin_table.password.ToString();

                var getEmployeeCategoty = ags.emp_category_table.Where(x => x.status == "publish").ToList();
                SelectList list = new SelectList(getEmployeeCategoty, "emp_category_id", "emp_category");
                ViewBag.categoryList = list;
                if (existing.photo == null && admin_table.ImageFile != null)
                {

                    string BigfileName = Path.GetFileNameWithoutExtension(admin_table.ImageFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension1 = Path.GetExtension(admin_table.ImageFile.FileName);
                    string extension = extension1.ToLower();
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
                        return RedirectToAction("back", "UserProfile");
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
                        string extension1 = Path.GetExtension(admin_table.ImageFile.FileName);
                        string extension = extension1.ToLower();
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
                            return RedirectToAction("back", "UserProfile");
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
                        //return PartialView("Edit", "SuperAdmin");
                        return RedirectToAction("back", "UserProfile");
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
                return RedirectToAction("back", "UserProfile");
            }
            return RedirectToAction("back", "UserProfile");
        }

        //public JsonResult passwordmatch(string password)
        //{
            
        //    string username = Session["username"].ToString();

        //    var admin_table = ags.admin_table.Where(x => x.username.ToString() == username).FirstOrDefault();
            

        //    bool result = PasswordStorage.VerifyPassword(password, admin_table.password);
        //    if (result)
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json("Sorry, this password is not correct", JsonRequestBehavior.AllowGet);
        //    }

          

           


        //}


 




        public ActionResult Password()
        {
            if (Session["username"] == null && Session["userid"] == null)
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var model = new agskeys.Models.ChangePassword();
            return PartialView(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(ChangePassword changePwd)
        {
            if (ModelState.IsValid)
            {
                string userid = Session["userid"].ToString();
                if(userid != null)
                {
                    admin_table existing = ags.admin_table.Where(x => x.id.ToString() == userid).FirstOrDefault();

                    var password = existing.password.ToString();
                    var oldPassword = changePwd.password;
                    var newPassword = changePwd.newpassword;
                    var retypePassword = changePwd.retypepassword;
                    bool result = PasswordStorage.VerifyPassword(oldPassword, password);
                    if (result)
                    {
                        if (newPassword == retypePassword)
                        {
                            existing.password = PasswordStorage.CreateHash(newPassword);
                        }
                        else
                        {
                            TempData["NotEqual"] = "<script>alert('password dosen't match');</script>";
                            return RedirectToAction("back", "UserProfile");
                        }
                    }
                    else
                    {
                        TempData["logAgain"] = "Oops.! Please Provide Valid Credentials.";
                        return RedirectToAction("Logout", "Account");
                    }
                    ags.SaveChanges();



                    return RedirectToAction("back", "UserProfile");


                }
                else
                {
                    TempData["logAgain"] = "Oops.! Something Went Wrong.";
                    return RedirectToAction("Logout", "Account");
                }                
            }
            return RedirectToAction("back", "UserProfile");
        }


        public ActionResult back()
        {
            if (Session["userlevel"].ToString() == "super_admin")
            {

                return RedirectToAction("Index", "SuperAdmin");

            }
            else if (Session["userlevel"].ToString() == "admin")
            {

                return RedirectToAction("Index", "Admin");

            }
            else if (Session["userlevel"].ToString() == "sales_executive")
            {

                return RedirectToAction("Customer", "SalesExecutive");

            }
            else if (Session["userlevel"].ToString() == "tele_marketing")
            {

                return RedirectToAction("Customer", "TeleMarketing");

            }
            else if (Session["userlevel"].ToString() == "process_team")
            {

                return RedirectToAction("Customer", "ProcessTeam");

            }
            else if (Session["userlevel"].ToString() == "process_executive")
            {

                return RedirectToAction("Customer", "ProcessExecutive");

            }
            else if (Session["userlevel"].ToString() == "manager")
            {

                return RedirectToAction("Customer", "Manager");

            }
            else
            {

                return RedirectToAction("Logout", "Account");

            }

        }

    }
}