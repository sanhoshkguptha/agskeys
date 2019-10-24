using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.MobileSalesExecutive
{
    [Authorize]
    public class MobileSalesExecutiveController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: MobileSalesExecutive
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            //var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();
            string userid = Session["userid"].ToString();
            var customers = (from s in ags.customer_profile_table
                             join sa in ags.loan_table on s.id.ToString() equals sa.customerid
                             join sb in ags.loan_track_table on sa.id.ToString() equals sb.loanid
                             where sb.employeeid == userid
                             orderby sb.datex descending
                             select s).Distinct().ToList();


            return View("~/Views/MobileSalesExecutive/MobileSalesExecutive/Customer.cshtml", customers);
        }
        public ActionResult Customer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            //var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();
            string userid = Session["userid"].ToString();
            var customers = (from s in ags.customer_profile_table
                             join sa in ags.loan_table on s.id.ToString() equals sa.customerid
                             join sb in ags.loan_track_table on sa.id.ToString() equals sb.loanid
                             where sb.employeeid == userid
                             orderby sb.datex descending
                             select s).Distinct().ToList();


            return View("~/Views/MobileSalesExecutive/MobileSalesExecutive/Customer.cshtml", customers);
        }

        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/MobileSalesExecutive/MobileSalesExecutive/Details.cshtml", user);
        }
        public ActionResult UserProfile()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive" || Session["userid"] == null)
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            else
            {
                var intId = Session["userid"].ToString();
                var Id = Convert.ToInt32(intId);
                var user = ags.admin_table.Where(x => x.id == Id).FirstOrDefault();
                if(user.userrole!=null)
                {
                    var employeeType = ags.emp_category_table.Where(x => x.emp_category_id == user.userrole).FirstOrDefault();
                    user.userrole = employeeType.emp_category;
                }
                else
                {
                    user.userrole = "Not Assigned";
                }
                
                return PartialView("~/Views/MobileSalesExecutive/MobileSalesExecutive/UserProfile.cshtml", user);
            }            

        }
        public ActionResult Support()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileSalesExecutive/MobileSalesExecutive/Support.cshtml");
        }





        public ActionResult EditProfile(int? Id)
        {
            if (Session["username"] == null && Session["userid"] == null)
            {
                return this.RedirectToAction("MobileLogout", "Account");
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
            return PartialView("~/Views/MobileSalesExecutive/MobileSalesExecutive/EditProfile.cshtml", admin_table);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(admin_table admin_table, FormCollection form)
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
                        return RedirectToAction("EditProfile", "MobileSalesExecutive");
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
                            return RedirectToAction("EditProfile", "MobileSalesExecutive");
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
                        TempData["Message"] = "This user name is already exist";
                        //return PartialView("Edit", "SuperAdmin");
                        return RedirectToAction("EditProfile", "MobileSalesExecutive");
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
                TempData["updateSuccess"] = "Your Profile Updated Successfully.!";
                return RedirectToAction("UserProfile", "MobileSalesExecutive");
            }
            return RedirectToAction("UserProfile", "MobileSalesExecutive");
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
            if (Session["username"] == null && Session["userid"] == null && Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            var model = new agskeys.Models.ChangePassword();
            return PartialView("~/Views/MobileSalesExecutive/MobileSalesExecutive/Password.cshtml", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(ChangePassword changePwd)
        {
            if (ModelState.IsValid)
            {
                string userid = Session["userid"].ToString();
                if (userid != null)
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
                            return RedirectToAction("UserProfile", "MobileSalesExecutive");
                        }
                    }
                    else
                    {
                        TempData["logAgain"] = "Oops.! Please Provide Valid Credentials.";
                        return this.RedirectToAction("MobileLogout", "Account");
                    }
                    ags.SaveChanges();
                    TempData["PswdSuccess"] = "Your Password Reset Successfully";
                    return RedirectToAction("UserProfile", "MobileSalesExecutive");


                }
                else
                {
                    TempData["logAgain"] = "Oops.! Something Went Wrong.";
                    return this.RedirectToAction("MobileLogout", "Account");
                }
            }
            return RedirectToAction("UserProfile", "MobileSalesExecutive");
        }
    }
}