  
using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;

namespace agskeys.Controllers
  
{
    [Authorize]
    public class SuperAdminController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();

        public ActionResult Index()
        {

            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var name = Session["username"].ToString();
            var getEmployeeCategoty = ags.emp_category_table.ToList();

            var customerCount = ags.customer_profile_table.ToList().Count();
            var partnerCount = ags.vendor_table.ToList().Count();
            var employeeCount = ags.admin_table.Where(t => t.userrole != "super_admin").ToList().Count();
            var superAdminCount = ags.admin_table.Where(t=>t.userrole == "super_admin").ToList().Count();
            var photo = ags.admin_table.Where(t => t.userrole == "super_admin" && t.username==name).ToList();

            ViewData["photo"] = photo.FirstOrDefault().photo;

            ViewData["customerCount"] = customerCount.ToString();
            ViewData["partnerCount"] = partnerCount.ToString();
            ViewData["employeeCount"] = employeeCount.ToString();
            ViewData["superAdminCount"] = superAdminCount.ToString();



            //// Current Month Transaction //

            //DateTime now = DateTime.Now;
            //var startDate = new DateTime(now.Year, now.Month, 1);

            //List<loan_table> currentmonth = new List<loan_table>();


            //// Full  Transaction //
            //var fulltrans = currentmonth;
            //ViewBag.fulltrans = fulltrans;

            //foreach (var items in ags.loan_table)
            //{
            //    DateTime dueDate = DateTime.ParseExact(items.datex, "dd / MM / yyyy HH:mm:ss", null);
            //    if ((dueDate >= startDate) && (dueDate <= now))
            //    {
            //        currentmonth.Add(items);
            //    }
            //}
            //ViewBag.currentmonth = currentmonth;



            // Current Month Transaction end //
            //////

            //int currentmonth = 0;
            //int currentmonthdisbursed = 0;
            //var lasttotalloanamt = ags.loan_table.ToList();

           
            //DateTime now2 = DateTime.Now;
           
            //string datecheck = now2.ToString();
            //CultureInfo culture = new CultureInfo("en-US");
            //DateTime dateTimeObj = Convert.ToDateTime(datecheck, culture);
            //string firstWord = dateTimeObj.ToString();
            //firstWord = firstWord.Split(' ').First();
            
            //DateTime now = DateTime.Parse(firstWord);
            //var startDate = new DateTime(now.Year, now.Month, 1);
            //foreach (var items in lasttotalloanamt)
            //{
            //    if(items.datex != null)
            //    {
            //        string datex = items.datex;
            //        DateTime dateTimeObj2 = Convert.ToDateTime(datex, culture);
            //        string firstWordloan = dateTimeObj2.ToString();
            //        firstWordloan = firstWordloan.Split(' ').First();
                  
            //        DateTime dueDate = DateTime.Parse(firstWordloan);
            //        if ((dueDate >= startDate) && (dueDate <= now))
            //        {
            //            currentmonth = currentmonth + int.Parse(items.loanamt);
            //            currentmonthdisbursed = currentmonthdisbursed + int.Parse(items.disbursementamt);

            //        }

            //    }

            //}
            //ViewBag.lastmonth = currentmonth;
            //ViewBag.currentmonthdisbursed = currentmonthdisbursed;
            ////////////////
            // total loan amount
            int loantotal = 0;
            var totalloanamt = ags.loan_table.ToList();
            foreach(var item in totalloanamt)
            {
                if(item.loanamt != null)
                {
                    loantotal = loantotal + int.Parse(item.loanamt);
                }                
            }            
            ViewBag.loanTotalAmt = Convert.ToDecimal(loantotal).ToString("#,##0.00");
            //total disbursed amount
            int loantotaldisbursed = 0;
            var totalloanamtdisbursed = ags.loan_table.ToList();
            foreach (var item in totalloanamtdisbursed)
            {
                if (item.disbursementamt != null)
                {
                    loantotaldisbursed = loantotaldisbursed + int.Parse(item.disbursementamt);
                }
            }
            ViewBag.loanTotalAmtDisbursed = Convert.ToDecimal(loantotaldisbursed).ToString("#,##0.00");
            return View();
        }

        


        public ActionResult Admin()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getEmployeeCategoty = ags.emp_category_table.ToList();
            var subAdmin = (from sub in ags.admin_table orderby sub.id descending select sub).ToList();
            //var userrole = "";
            //foreach (var item in subAdmin)
            //{                
            //    foreach(var items in getEmployeeCategoty)
            //    {
            //        if (items.id.ToString() == item.userrole)
            //        {
            //            userrole = items.emp_category +" ("+ items.status + ")";
            //            break;
            //        }
            //        else if (items.id.ToString() != item.userrole)
            //        {
            //            userrole = "Not Updated";
            //            continue;
            //        }
            //    }
            //    item.userrole = userrole;
            //}
            foreach (var item in subAdmin)
            {
                foreach (var items in getEmployeeCategoty)
                {
                    if (items.emp_category_id.ToString() == item.userrole)
                    {
                        string concatenated = items.emp_category + " ( " + items.status + " ) ";
                        item.userrole = concatenated;
                        break;
                    }
                    else if (!ags.emp_category_table.Any(s => s.emp_category_id.ToString() == item.userrole))
                    {
                        item.userrole = "Not Updated";
                    }
                }

            }

            return PartialView(subAdmin);

        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getEmployeeCategoty = ags.emp_category_table.Where(x => x.status == "publish").ToList();
            SelectList list = new SelectList(getEmployeeCategoty, "emp_category_id", "emp_category");
            ViewBag.categoryList = list;
            var model = new agskeys.Models.admin_table();
            return PartialView(model);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(admin_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                var usr = (from u in ags.admin_table where u.username == obj.username select u).FirstOrDefault();
                var allowedExtensions = new[] {
                    ".png", ".jpg", ".jpeg"
                };

                if (usr == null)
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
                    return RedirectToAction("Admin");
                }
            }
            return View(obj);
        }


        //public JsonResult UsernameExists(string username)
        //{
        //    //var model = new agskeys.Models.proof_table();
        //    var admin_table = ags.admin_table.ToList();
        //    var userdata = "";
        //    foreach (var items in admin_table)
        //    {
        //        if (username == items.username)
        //        {
        //            userdata = items.username;
        //            break;
        //        }
        //        else
        //        {
        //            userdata = items.username;
        //            continue;
        //        }
            

        //    }
        //    return Json(!String.Equals(username, userdata, StringComparison.OrdinalIgnoreCase));

        //}




        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin" )
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
        

            return PartialView(user);
        }

        //public ActionResult UserProfile()
        //{
        //    if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
        //    {
        //        return this.RedirectToAction("Logout", "Account");
        //    }
        //    else
        //    {
        //        var intId = Session["userid"].ToString();
        //        var Id = Convert.ToInt32(intId);
        //        var user = ags.admin_table.Where(x => x.id == Id).FirstOrDefault();
        //        return PartialView(user);
        //    }
        //    return View();

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
                        //return PartialView("Edit", "SuperAdmin");
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
                return RedirectToAction("Admin", "SuperAdmin");
            }
            return RedirectToAction("Admin", "SuperAdmin");
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
            return PartialView(user);
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














// if (Session["username"] == null)
//            {
//                RedirectToAction("Login");
//            }            
//            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
//var getCustomerProfile = ags.customer_profile_table.Where(x => x.id.ToString() == user.customerid.ToString()).ToList();
//SelectList customers = new SelectList(getCustomerProfile, "id", "customerid", "name", "phoneno", "profileimg");
//ViewBag.customerList = customers;
           
//            return PartialView(user);