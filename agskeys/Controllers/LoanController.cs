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
    public class LoanController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Loan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getCustomer = ags.customer_profile_table.ToList();
            var customer_loans = (from loan_table in ags.loan_table orderby loan_table.id descending select loan_table).ToList();
            var customerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getCustomer)
                {
                    if (item.customerid.ToString() == items.id.ToString())
                    {
                        string concatenated = items.name.ToString() +" ( " + items.customerid + " ) ";
                        customerid = concatenated;
                        break;
                       
                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        customerid = "Not Updated";
                        continue;
                    }                   
                }
                item.employeetype = customerid;

            }


            var getVendor = ags.vendor_table.ToList();
            var partnerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getVendor)
                {
                    if (item.partnerid == items.id.ToString())
                    {
                        partnerid = items.companyname;
                        break;
                    }
                    else if (items.id.ToString() != item.partnerid)
                    {
                        partnerid = "Not Updated";
                        continue;
                    }                  

                }
                item.employee = partnerid;
            }

            var getloantype = ags.loantype_table.ToList();
            foreach (var item in customer_loans)
            {
                foreach (var items in getloantype)
                {
                    if (item.loantype == items.id.ToString())
                    {
                        item.loantype = items.loan_type;
                        break;
                    }
                    else if (!ags.loan_table.Any(s => s.loantype.ToString() == items.id.ToString()))
                    {
                        item.loantype = "Not Updated";
                    }
                }
            }
            
            //// for handled by option
            foreach (var item in customer_loans)
            {
                var getHandledBy = ags.loan_track_table.Where(x=>x.loanid == item.id.ToString()).ToList();
                
            }
            return PartialView(customer_loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Loan(FormCollection form)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getCustomer = ags.customer_profile_table.ToList();

            string SearchFor = "";

            var customer_loans = (from loan_table in ags.loan_table orderby loan_table.id descending select loan_table).ToList();

            if (form != null)
            {
                if (form["ongoing"] != null)
                {
                    SearchFor = form["ongoing"].ToString();
                }
                else if (form["Partialydisbursed"] != null)
                {
                    SearchFor = form["Partialydisbursed"].ToString();
                }
                else if (form["fullydisbursed"] != null)
                {
                    SearchFor = form["fullydisbursed"].ToString();
                }

                if (SearchFor == "ongoing")
                {
                    customer_loans = (from s in ags.loan_table
                                      join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                      where (s.disbursementamt == "0")
                                      orderby sa.datex descending
                                      select s).Distinct().ToList();

                }
                else if (SearchFor == "Partialydisbursed")
                {
                    customer_loans = (from s in ags.loan_table
                                      join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                      where (s.disbursementamt != s.loanamt && s.disbursementamt != "0")
                                      orderby sa.datex descending
                                      select s).Distinct().ToList();


                }
                else if (SearchFor == "fullydisbursed")
                {
                    customer_loans = (from s in ags.loan_table
                                      join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                      where (s.disbursementamt == s.loanamt && s.requestloanamt != "0" && s.loanamt != "0")
                                      orderby sa.datex descending
                                      select s).Distinct().ToList();
                }

            }

            var customerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getCustomer)
                {
                    if (item.customerid.ToString() == items.id.ToString())
                    {
                        string concatenated = items.name.ToString() + " ( " + items.customerid + " ) ";
                        customerid = concatenated;
                        break;

                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        customerid = "Not Updated";
                        continue;
                    }
                }
                item.employeetype = customerid;

            }


            var getVendor = ags.vendor_table.ToList();
            var partnerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getVendor)
                {
                    if (item.partnerid == items.id.ToString())
                    {
                        partnerid = items.companyname;
                        break;
                    }
                    else if (items.id.ToString() != item.partnerid)
                    {
                        partnerid = "Not Updated";
                        continue;
                    }

                }
                item.employee = partnerid;
            }

            var getloantype = ags.loantype_table.ToList();
            foreach (var item in customer_loans)
            {
                foreach (var items in getloantype)
                {
                    if (item.loantype == items.id.ToString())
                    {
                        item.loantype = items.loan_type;
                        break;
                    }
                    else if (!ags.loan_table.Any(s => s.loantype.ToString() == items.id.ToString()))
                    {
                        item.loantype = "Not Updated";
                    }
                }
            }

            //// for handled by option
            foreach (var item in customer_loans)
            {
                var getHandledBy = ags.loan_track_table.Where(x => x.loanid == item.id.ToString()).ToList();

            }
            return PartialView(customer_loans);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getCustomer = ags.customer_profile_table.ToList();
            SelectList customers = new SelectList(getCustomer, "id", "customerid");
            ViewBag.customerList = customers;

            var getVendor = ags.vendor_table.ToList();
            SelectList vendors = new SelectList(getVendor, "id", "companyname");
            ViewBag.vendorList = vendors;

            var getBank = ags.bank_table.ToList();
            SelectList banks = new SelectList(getBank, "id", "bankname");
            ViewBag.bankList = banks;

            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;

            var empCategory = ags.emp_category_table.Where(x=>x.emp_category_id!="admin" && x.emp_category_id != "super_admin" && x.emp_category_id != "clientele" && x.emp_category_id != "partner").ToList();
            SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
            ViewBag.empCategories = empCategories;

            var employee = ags.admin_table.ToList();
            SelectList employees = new SelectList(employee, "id", "name");
            ViewBag.employees = employees;

            var ExtComment = ags.external_comment_table.ToList();
            SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
            ViewBag.commentList = commentlist;


            var model = new agskeys.Models.loan_table();
            return PartialView(model);
        }


        public JsonResult GetEmployeeList(string categoryId)
        {
            ags.Configuration.ProxyCreationEnabled = false;
            List<admin_table> employees = ags.admin_table.Where(x => x.userrole.ToString() == categoryId).ToList();
            return Json(employees, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(loan_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                var getCustomer = ags.customer_profile_table.ToList();
                SelectList customers = new SelectList(getCustomer, "id", "customerid");
                ViewBag.customerList = customers;

                var getVendor = ags.vendor_table.ToList();
                SelectList vendors = new SelectList(getVendor, "id", "companyname");
                ViewBag.vendorList = vendors;

                var getBank = ags.bank_table.ToList();
                SelectList banks = new SelectList(getBank, "id", "bankname");
                ViewBag.bankList = banks;
                
                var getloantype = ags.loantype_table.ToList();
                SelectList loantp = new SelectList(getloantype, "id", "loan_type");
                ViewBag.loantypeList = loantp;

                var empCategory = ags.emp_category_table.Where(x => x.emp_category_id != "admin" && x.emp_category_id != "super_admin" && x.emp_category_id != "clientele" && x.emp_category_id != "partner").ToList();
                SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
                ViewBag.empCategories = empCategories;

                var employee = ags.admin_table.ToList();
                SelectList employees = new SelectList(employee, "id", "name");
                ViewBag.employees = employees;

                var ExtComment = ags.external_comment_table.ToList();
                SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
                ViewBag.commentList = commentlist;

                // var usr = (from u in ags.loan_table where u. == obj.username select u).FirstOrDefault();
                var allowedExtensions = new[] {
                    ".png", ".jpg", ".jpeg",".doc",".docx",".pdf"
                };
                if (obj.sactionedCopyFile != null)
                {
                    string sactionedFileName = Path.GetFileNameWithoutExtension(obj.sactionedCopyFile.FileName);
                    string fileName = sactionedFileName.Substring(0, 1);
                    string extension1 = Path.GetExtension(obj.sactionedCopyFile.FileName);
                    string extension = extension1.ToLower();
                    if (allowedExtensions.Contains(extension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                        obj.sactionedcopy = "~/sactionedcopyfile/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/sactionedcopyfile/"), fileName);
                        obj.sactionedCopyFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg','png','jpeg','docx','doc','pdf' images formats are alllowed..!";
                        return RedirectToAction("Loan");
                    }
                }
                if (obj.idCopyFile != null)
                {
                    string idCopyFileName = Path.GetFileNameWithoutExtension(obj.idCopyFile.FileName);
                    string idFileName = idCopyFileName.Substring(0, 1);
                    string extension2 = Path.GetExtension(obj.idCopyFile.FileName);
                    string idExtension = extension2.ToLower();
                    if (allowedExtensions.Contains(idExtension))
                    {
                        idFileName = idFileName + DateTime.Now.ToString("yyssmmfff") + extension2;
                        obj.idcopy = "~/idcopyfile/" + idFileName;
                        idFileName = Path.Combine(Server.MapPath("~/idcopyfile/"), idFileName);
                        obj.idCopyFile.SaveAs(idFileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg','png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("Loan");
                    }
                }
                if (obj.propertyDocumentsFile != null)
                {
                    string propertyDocumentsFile = Path.GetFileNameWithoutExtension(obj.propertyDocumentsFile.FileName);
                    string propertyFileName = propertyDocumentsFile.Substring(0, 1);
                    string extension3 = Path.GetExtension(obj.propertyDocumentsFile.FileName);
                    string propertyExtension = extension3.ToLower();
                    if (allowedExtensions.Contains(propertyExtension))
                    {
                        propertyFileName = propertyFileName + DateTime.Now.ToString("yyssmmfff") + extension3;
                        obj.propertydocuments = "~/propertyFile/" + propertyFileName;
                        propertyFileName = Path.Combine(Server.MapPath("~/propertyFile/"), propertyFileName);
                        obj.propertyDocumentsFile.SaveAs(propertyFileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg','png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("Loan");
                    }
                }
                loan_table loan = new loan_table();
                loan.customerid = obj.customerid;
                loan.partnerid = obj.partnerid;
                loan.bankid = obj.bankid;
                loan.loantype = obj.loantype;
                if (!string.IsNullOrEmpty(obj.loanamt))
                {
                    loan.loanamt = obj.loanamt;                    
                }
                else
                {
                    loan.loanamt = "0";
                }
                loan.requestloanamt = obj.requestloanamt;
                if (!string.IsNullOrEmpty(obj.disbursementamt))
                {
                    loan.disbursementamt = obj.disbursementamt;                   
                }
                else
                {
                    loan.disbursementamt = "0";
                }                
                loan.rateofinterest = obj.rateofinterest;
                loan.sactionedcopy = obj.sactionedcopy;
                loan.idcopy = obj.idcopy;
                loan.propertydocuments = obj.propertydocuments;
                loan.propertydetails = obj.propertydetails;
                if (!string.IsNullOrEmpty(obj.loanstatus))
                {
                    loan.loanstatus = obj.loanstatus;
                }
                else
                {
                    loan.loanstatus = "Pending";
                }                
                loan.datex = DateTime.Now.ToString();
                loan.addedby = Session["username"].ToString();
                ags.loan_table.Add(loan);
                ags.SaveChanges();

                int latestloanid = loan.id;

                loan_track_table loan_track = new loan_track_table();
                loan_track.loanid = latestloanid.ToString(); 
                if (Session["userid"] != null)
                {
                    loan_track.employeeid = Session["userid"].ToString();
                    loan_track.tracktime = DateTime.Now.ToString();
                }
                //if (obj.partnerid != null)
                //{
                //    loan_track.vendorid = obj.partnerid;
                //    loan_track.vendortracktime = DateTime.Now.ToString();

                //}
                if (obj.internalcomment != null)
                {
                    loan_track.internalcomment = obj.internalcomment;
                }
                if (obj.externalcomment != null)
                {
                    loan_track.externalcomment = obj.externalcomment;
                }
                loan_track.datex = DateTime.Now.ToString();
                loan_track.addedby = Session["username"].ToString();
                ags.loan_track_table.Add(loan_track);
                ags.SaveChanges();


                ///Assigned Employee
                
                loan_track_table loan_track_employee = new loan_track_table();
                if (obj.employee != null)
                {
                    loan_track_employee.loanid = latestloanid.ToString();
                    loan_track_employee.employeeid = obj.employee;
                    loan_track_employee.followupdate = obj.followupdate;
                    loan_track_employee.tracktime = DateTime.Now.ToString();
                    //if (obj.partnerid != null)
                    //{
                    //    loan_track.vendorid = obj.partnerid;
                    //    loan_track.vendortracktime = DateTime.Now.ToString();

                    //}
                    loan_track_employee.internalcomment = "Assigned";
                    loan_track_employee.externalcomment = "Assigned";
                    
                    loan_track_employee.datex = DateTime.Now.ToString();
                    loan_track_employee.addedby = Session["username"].ToString();
                    ags.loan_track_table.Add(loan_track_employee);
                    ags.SaveChanges();
                }              
               

                vendor_track_table vendor_track = new vendor_track_table();
                if(obj.partnerid != null)
                {
                    vendor_track.loanid = latestloanid.ToString();
                    vendor_track.vendorid = obj.partnerid;
                    vendor_track.tracktime = DateTime.Now.ToString();
                    vendor_track.comment = "Assigned";
                    vendor_track.datex = DateTime.Now.ToString();
                    vendor_track.addedby = Session["username"].ToString();
                    ags.vendor_track_table.Add(vendor_track);
                    ags.SaveChanges();

                }
                

                //assigned table

                assigned_table assigned = new assigned_table();
                assigned.loanid = latestloanid.ToString();
                if (obj.employee != null)
                {
                    assigned.assign_emp_id = obj.employee;
                }
                else
                {
                    assigned.assign_emp_id = Session["userid"].ToString();
                }
                if (obj.partnerid != null)
                {
                    assigned.assign_vendor_id = obj.partnerid;
                }               
                assigned.datex = DateTime.Now.ToString();
                assigned.addedby = Session["username"].ToString();
                ags.assigned_table.Add(assigned);

                // Loan notification to Super admin
                ags.notification_table.Add(new notification_table
                {
                    notification = "New Loan has Created for " + obj.customerid + " By Super Admin",
                    seenstatus = 1,
                    userid = "super_admin",
                    addedby = Session["username"].ToString(),
                    datex = DateTime.Now.ToString(),
                });

                ags.SaveChanges();


                return RedirectToAction("Loan");

            }
            else
            {
                TempData["AE"] = "Something went wrong";
                return RedirectToAction("Loan");
            }
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
            //var getCustomerProfile = ags.customer_profile_table.Where(x=>x.id.ToString() == user.customerid.ToString()).ToList();           
            //SelectList customers = new SelectList(getCustomerProfile, "id", "customerid", "name", "phoneno", "profileimg");
            //ViewBag.customerList = customers;

            var getCustomer = ags.customer_profile_table.ToList();

            string id = "";
            string name = "";
            string phone = "";
            string email = "";
            string profilimg = "";            

            foreach (var customer in getCustomer)
            {
                if (user.customerid == customer.id.ToString())
                {
                    id = customer.customerid.ToString();
                    name = customer.name;
                    phone = customer.phoneno;
                    email = customer.email;
                    profilimg = customer.profileimg;
                    break;
                }
                else if (user.customerid != customer.id.ToString())
                {
                    id = "Not Updated";
                    name = "Not Updated";
                    phone = "Not Updated";
                    email = "Not Updated";
                    profilimg = "Not Updated";
                }
            }
            user.customerid = id;
            ViewBag.name = name;
            ViewBag.phoneno = phone;
            ViewBag.email = email;
            ViewBag.profileimg = profilimg;


            var getVendor = ags.vendor_table.ToList();
            string partner = "";
            foreach (var items in getVendor)
            {
                if (user.partnerid == items.id.ToString())
                {
                    string concatenated = items.companyname + " ( Company Name ) ";
                    partner = concatenated;
                    break;
                }
                else if (user.partnerid != items.id.ToString())
                {
                    partner = "Not Updated" + " ( Company Name ) ";
                }
            }
            user.partnerid = partner;

            var getBank = ags.bank_table.ToList();
            string banknm = "";
            foreach (var bank in getBank)
            {
                if (user.bankid == bank.id.ToString())
                {
                    banknm = bank.bankname;
                    break;
                }
                else if (user.bankid != bank.id.ToString())
                {
                    banknm = "Not Updated";
                }
            }
            user.bankid = banknm;

            var getloantype = ags.loantype_table.ToList();
            string loan = "";
            foreach (var loantp in getloantype)
            {
                if (user.loantype == loantp.id.ToString())
                {
                    loan = loantp.loan_type;
                    break;
                }
                else if (user.loantype != loantp.id.ToString())
                {
                    loan = "Not Updated";
                }
            }
            user.loantype = loan;

            string loanid = Id.ToString();
            int loan_count = ags.process_executive.Where(x => x.loanid == loanid).Count();
            if (loan_count == 1)
            {
                process_executive process_executive = ags.process_executive.Where(x => x.loanid == loanid).FirstOrDefault();

                ViewBag.loan_count = loan_count;
                ViewBag.technical = process_executive.technical;
                ViewBag.legal = process_executive.legal;
                ViewBag.rcu = process_executive.rcu;
                if (process_executive.comment == null)
                {
                    process_executive.comment = "0";
                    ViewBag.comment = process_executive.comment;
                }
                else
                {
                    ViewBag.comment = process_executive.comment;
                }
            }

            return PartialView(user);
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

            var getCustomer = ags.customer_profile_table.ToList();
            SelectList customers = new SelectList(getCustomer, "id", "customerid");
            ViewBag.customerList = customers;

            var getVendor = ags.vendor_table.ToList();
            SelectList vendors = new SelectList(getVendor, "id", "companyname");
            ViewBag.vendorList = vendors;

            var getBank = ags.bank_table.ToList();
            SelectList banks = new SelectList(getBank, "id", "bankname");
            ViewBag.bankList = banks;

            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;

            var empCategory = ags.emp_category_table.Where(x => x.emp_category_id != "admin" && x.emp_category_id != "super_admin" && x.emp_category_id != "clientele" && x.emp_category_id != "partner").ToList();
            //var empCategory = ags.emp_category_table.ToList();
            SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
            ViewBag.empCategories = empCategories;

            var employee = ags.admin_table.ToList();
            SelectList employees = new SelectList(employee, "id", "name");
            ViewBag.employees = employees;

            var ExtComment = ags.external_comment_table.ToList();
            SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
            ViewBag.commentList = commentlist;

            //List<emp_category_table> categoryList = ags.emp_category_table.ToList();
            //ViewBag.empCategories = new SelectList(categoryList, "emp_category_id", "emp_category");


            loan_table loan_table = ags.loan_table.Find(Id);
            if (loan_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(loan_table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(loan_table loan_table)
        {
            if (ModelState.IsValid)
            {
                var getCustomer = ags.customer_profile_table.ToList();
                SelectList customers = new SelectList(getCustomer, "id", "customerid");
                ViewBag.customerList = customers;

                var getVendor = ags.vendor_table.ToList();
                SelectList vendors = new SelectList(getVendor, "id", "companyname");
                ViewBag.vendorList = vendors;

                var getBank = ags.bank_table.ToList();
                SelectList banks = new SelectList(getBank, "id", "bankname");
                ViewBag.bankList = banks;

                var getloantype = ags.loantype_table.ToList();
                SelectList loantp = new SelectList(getloantype, "id", "loan_type");
                ViewBag.loantypeList = loantp;

                var ExtComment = ags.external_comment_table.ToList();
                SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
                ViewBag.commentList = commentlist;

                var empCategory = ags.emp_category_table.Where(x => x.emp_category_id != "admin" && x.emp_category_id != "super_admin" && x.emp_category_id != "clientele" && x.emp_category_id != "partner").ToList();
                //var empCategory = ags.emp_category_table.ToList();
                SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
                ViewBag.empCategories = empCategories;


                var allowedExtensions = new[] {
                    ".png", ".jpg", ".jpeg",".doc",".docx",".pdf"
                };
                loan_table existing = ags.loan_table.Find(loan_table.id);
                string partner = existing.partnerid;
                
                if (existing.sactionedcopy == null && loan_table.sactionedCopyFile != null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(loan_table.sactionedCopyFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension2 = Path.GetExtension(loan_table.sactionedCopyFile.FileName);
                    string sactionedExtension = extension2.ToLower();
                    if (allowedExtensions.Contains(sactionedExtension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + sactionedExtension;
                        loan_table.sactionedcopy = "~/sactionedcopyfile/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/sactionedcopyfile/"), fileName);
                        loan_table.sactionedCopyFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("Loan");
                    }
                }


                else if (existing.sactionedcopy != null && loan_table.sactionedcopy != null)
                {
                    if (loan_table.sactionedCopyFile != null)
                    {
                        string path = Server.MapPath(existing.sactionedcopy);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(loan_table.sactionedCopyFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension2 = Path.GetExtension(loan_table.sactionedCopyFile.FileName);
                        string sactionedExtension = extension2.ToLower();
                        if (allowedExtensions.Contains(sactionedExtension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + sactionedExtension;
                            loan_table.sactionedcopy = "~/sactionedcopyfile/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/sactionedcopyfile/"), fileName);
                            loan_table.sactionedCopyFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Loan");
                        }

                    }
                    else
                    {
                        existing.sactionedcopy = existing.sactionedcopy;
                    }
                }
                else
                {
                    existing.sactionedcopy = existing.sactionedcopy;
                }

                //property documents

                if (existing.propertydocuments == null && loan_table.propertyDocumentsFile != null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(loan_table.propertyDocumentsFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension2 = Path.GetExtension(loan_table.propertyDocumentsFile.FileName);
                    string propertyExtension = extension2.ToLower();
                    if (allowedExtensions.Contains(propertyExtension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + propertyExtension;
                        loan_table.propertydocuments = "~/propertyFile/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/propertyFile/"), fileName);
                        loan_table.propertyDocumentsFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("Loan");
                    }
                }


                else if (existing.propertydocuments != null && loan_table.propertydocuments != null)
                {
                    if (loan_table.propertyDocumentsFile != null)
                    {
                        string path = Server.MapPath(existing.propertydocuments);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(loan_table.propertyDocumentsFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension2 = Path.GetExtension(loan_table.propertyDocumentsFile.FileName);
                        string propertyExtension = extension2.ToLower();
                        if (allowedExtensions.Contains(propertyExtension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + propertyExtension;
                            loan_table.idcopy = "~/propertyFile/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/propertyFile/"), fileName);
                            loan_table.propertyDocumentsFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Loan");
                        }

                    }
                    else
                    {
                        existing.propertydocuments = existing.propertydocuments;
                    }
                }
                else
                {
                    existing.propertydocuments = existing.propertydocuments;
                }

                //DD Copy

                if (existing.idcopy == null && loan_table.idCopyFile != null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(loan_table.idCopyFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension1 = Path.GetExtension(loan_table.idCopyFile.FileName);
                    string idExtension = extension1.ToLower();
                    if (allowedExtensions.Contains(idExtension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + idExtension;
                        loan_table.idcopy = "~/idcopyfile/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/idcopyfile/"), fileName);
                        loan_table.idCopyFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("Loan");
                    }
                }


                else if(existing.idcopy != null && loan_table.idcopy != null)
                {
                    if (loan_table.idCopyFile != null)
                    {
                        string path = Server.MapPath(existing.idcopy);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(loan_table.idCopyFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension1 = Path.GetExtension(loan_table.idCopyFile.FileName);
                        string idExtension = extension1.ToLower();
                        if (allowedExtensions.Contains(idExtension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + idExtension;
                            loan_table.idcopy = "~/idcopyfile/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/idcopyfile/"), fileName);
                            loan_table.idCopyFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Loan");
                        }

                    }
                    else
                    {
                        existing.idcopy = existing.idcopy;
                    }
                }
                else
                {
                    existing.idcopy = existing.idcopy;
                }

                existing.customerid = loan_table.customerid;
                existing.partnerid = loan_table.partnerid;
                existing.bankid = loan_table.bankid;
                existing.loantype = loan_table.loantype;
                existing.requestloanamt = loan_table.requestloanamt;
                existing.loanamt = loan_table.loanamt;
                existing.disbursementamt = loan_table.disbursementamt;
                existing.rateofinterest = loan_table.rateofinterest;
                existing.sactionedcopy = loan_table.sactionedcopy;
                existing.idcopy = loan_table.idcopy;
                existing.propertydocuments = loan_table.propertydocuments;
                existing.propertydetails = loan_table.propertydetails;
                //existing.followupdate = loan_table.followupdate;
                existing.loanstatus = loan_table.loanstatus;

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

                int latestloanid = loan_table.id;

                ///Assigned Employee
                loan_track_table loan_track_employee = new loan_track_table();
                if (loan_table.employee != null)
                {
                    loan_track_employee.loanid = latestloanid.ToString();
                    loan_track_employee.employeeid = loan_table.employee;
                    loan_track_employee.tracktime = DateTime.Now.ToString();

                    if (loan_table.internalcomment != null)
                    {
                        loan_track_employee.internalcomment = loan_table.internalcomment;
                    }
                    if (loan_table.externalcomment != null)
                    {
                        loan_track_employee.externalcomment = loan_table.externalcomment;
                    }

                    loan_track_employee.datex = DateTime.Now.ToString();
                    loan_track_employee.followupdate = loan_table.followupdate;
                    loan_track_employee.addedby = Session["username"].ToString();
                    ags.loan_track_table.Add(loan_track_employee);
                    ags.SaveChanges();
                }
                

                vendor_track_table vendor_track = new vendor_track_table();
                if (loan_table.partnerid != null)
                {
                    vendor_track.loanid = latestloanid.ToString();
                    if (loan_table.partnerid != partner)
                    {

                        vendor_track.vendorid = loan_table.partnerid;
                        vendor_track.tracktime = DateTime.Now.ToString();
                        vendor_track.comment = "Assigned";

                        vendor_track.datex = DateTime.Now.ToString();
                        vendor_track.addedby = Session["username"].ToString();
                        ags.vendor_track_table.Add(vendor_track);
                        ags.SaveChanges();

                    }
                }
                
                

                //assigned table
                assigned_table existing_data = ags.assigned_table.Where(x=>x.loanid == loan_table.id.ToString()).FirstOrDefault();


                //existing_data.loanid = latestloanid.ToString();
                if (loan_table.employee != null)
                {
                    existing_data.assign_emp_id = loan_table.employee;
                }
                else
                {
                    existing_data.assign_emp_id = Session["userid"].ToString();
                }
                if (loan_table.partnerid != null)
                {
                    existing_data.assign_vendor_id = loan_table.partnerid;
                }
                else
                {
                    existing_data.assign_vendor_id = Session["userid"].ToString();
                }
                if (existing_data.addedby == null)
                {
                    existing_data.addedby = Session["username"].ToString();
                }
                else
                {
                    existing_data.addedby = existing_data.addedby;
                }
                if (existing_data.datex == null)
                {
                    existing_data.datex = DateTime.Now.ToString();
                }
                else
                {
                    existing_data.datex = existing_data.datex;
                }
                //loan assingned to notification table
                ags.notification_table.Add(new notification_table
                {
                    notification = "Loan " + loan_table.id + " Assigned" + "to" + loan_table.employee,
                    seenstatus = 1,
                    userid = "super_admin",
                    addedby = Session["username"].ToString(),
                    datex = DateTime.Now.ToString(),
                });
                ags.SaveChanges();

                return RedirectToAction("Loan", "Loan");
            }
            return PartialView(loan_table);
        }





        public ActionResult Delete(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
            //var getCustomerProfile = ags.customer_profile_table.Where(x=>x.id.ToString() == user.customerid.ToString()).ToList();           
            //SelectList customers = new SelectList(getCustomerProfile, "id", "customerid", "name", "phoneno", "profileimg");
            //ViewBag.customerList = customers;

            var getCustomer = ags.customer_profile_table.ToList();

            string id = "";
            string name = "";
            string phone = "";
            string email = "";
            string profilimg = "";

            foreach (var customer in getCustomer)
            {
                if (user.customerid == customer.id.ToString())
                {
                    id = customer.customerid.ToString();
                    name = customer.name;
                    phone = customer.phoneno;
                    email = customer.email;
                    profilimg = customer.profileimg;
                    break;
                }
                else if (user.customerid != customer.id.ToString())
                {
                    id = "Not Updated";
                    name = "Not Updated";
                    phone = "Not Updated";
                    email = "Not Updated";
                    profilimg = "Not Updated";
                }
            }
            user.customerid = id;
            ViewBag.name = name;
            ViewBag.phoneno = phone;
            ViewBag.email = email;
            ViewBag.profileimg = profilimg;


            var getVendor = ags.vendor_table.ToList();
            string partner = "";
            foreach (var items in getVendor)
            {
                if (user.partnerid == items.id.ToString())
                {
                    string concatenated = items.companyname + " ( Company Name ) ";
                    partner = concatenated;
                    break;
                }
                else if (user.partnerid != items.id.ToString())
                {
                    partner = "Not Updated" + " ( Company Name ) ";
                }
            }
            user.partnerid = partner;

            var getBank = ags.bank_table.ToList();
            string banknm = "";
            foreach (var bank in getBank)
            {
                if (user.bankid == bank.id.ToString())
                {
                    banknm = bank.bankname;
                    break;
                }
                else if (user.bankid != bank.id.ToString())
                {
                    banknm = "Not Updated";
                }
            }
            user.bankid = banknm;

            var getloantype = ags.loantype_table.ToList();
            string loan = "";
            foreach (var loantp in getloantype)
            {
                if (user.loantype == loantp.id.ToString())
                {
                    loan = loantp.loan_type;
                    break;
                }
                else if (user.loantype != loantp.id.ToString())
                {
                    loan = "Not Updated";
                }
            }
            user.loantype = loan;

            string loanid = Id.ToString();
            int loan_count = ags.process_executive.Where(x => x.loanid == loanid).Count();
            if (loan_count == 1)
            {
                process_executive process_executive = ags.process_executive.Where(x => x.loanid == loanid).FirstOrDefault();

                ViewBag.loan_count = loan_count;
                ViewBag.technical = process_executive.technical;
                ViewBag.legal = process_executive.legal;
                ViewBag.rcu = process_executive.rcu;
            }

            return PartialView(user);
        }
        // POST: vendor_table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            loan_table loan_table = ags.loan_table.Find(id);
            string idcopypath = Server.MapPath(loan_table.idcopy);
            FileInfo fileIdCopy = new FileInfo(idcopypath);
            if (fileIdCopy.Exists)
            {
                fileIdCopy.Delete();
            }
            string sactionedcopypath = Server.MapPath(loan_table.sactionedcopy);
            FileInfo fileSactioned = new FileInfo(sactionedcopypath);
            if (fileSactioned.Exists)
            {
                fileSactioned.Delete();
            }
            var loan_track = ags.loan_track_table.Where(x => x.loanid == loan_table.id.ToString());
            ags.loan_track_table.RemoveRange(loan_track);
            var vendor_track = ags.vendor_track_table.Where(x => x.loanid == loan_table.id.ToString());
            ags.vendor_track_table.RemoveRange(vendor_track);
            var assigned = ags.assigned_table.Where(x => x.loanid == loan_table.id.ToString());
            ags.assigned_table.RemoveRange(assigned);

            int loan_count = ags.process_executive.Where(x => x.loanid == loan_table.id.ToString()).Count();
            if(loan_count != 0)
            {
                var process_loan = ags.process_executive.Where(x => x.loanid == loan_table.id.ToString());
                ags.process_executive.RemoveRange(process_loan);
            }
            ags.loan_table.Remove(loan_table);
            ags.SaveChanges();

            return RedirectToAction("Loan");
        }




        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<loan_table> loan = ags.loan_table.Where(x => x.id == Id).ToList();            
            List<loan_track_table> employeeLoantrack = ags.loan_track_table.Where(x => x.loanid == Id.ToString()).ToList();
            List<vendor_track_table> vendorLoantrack = ags.vendor_track_table.Where(x => x.loanid == Id.ToString()).ToList();
            List<external_comment_table> externalComment = ags.external_comment_table.ToList();

            var employee = ags.admin_table.ToList(); 
            loan_track loan_track = new loan_track();
            loan_track.loan_details = loan.ToList();
            loan_track.employee_track = employeeLoantrack.ToList().OrderBy(t => t.tracktime);
            loan_track.vendor_track = vendorLoantrack.ToList().OrderBy(t => t.tracktime);

            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
            var getCustomer = ags.customer_profile_table.ToList();
            var customerid = "";
            var phonenumber = "";
            var name = "";
            var email = "";
            foreach (var customer in getCustomer)
            {
                if (user.customerid == customer.id.ToString())
                {
                    name = customer.name;
                    customerid = customer.customerid;
                    phonenumber = customer.phoneno;
                    email = customer.email;
                    break;
                }
                else if (user.customerid != customer.id.ToString())
                {
                    customerid = "Not Updated";
                    continue;
                }
            } 
            user.customerid = customerid;
            ViewBag.name = name;
            ViewBag.phoneno = phonenumber;
            ViewBag.email = email;

            var employees = ags.admin_table.ToList();

            var employeeid = "";
            foreach (var item in employeeLoantrack)
            {
                foreach (var items in employees)
                {
                    if (item.employeeid != null)
                    {
                        if (item.employeeid.ToString() == items.id.ToString())
                        {
                            string concatenated = items.name + " ( " + items.userrole + " ) ";
                            employeeid = concatenated;
                            break;
                        }
                        else if (items.id.ToString() != item.employeeid)
                        {
                            employeeid = "Not Updated";
                            continue;
                        }
                    }
                }
                item.employeeid = employeeid;
            }

            var extComment = "";
            foreach (var item in employeeLoantrack)
            {
                foreach (var items in externalComment)
                {
                    if (item.externalcomment != null)
                    {
                        if (item.externalcomment.ToString() == items.id.ToString())
                        {
                            extComment = items.externalcomment;                          
                            break;
                        }
                        else if (items.id.ToString() != item.externalcomment)
                        {
                            extComment = "Not Updated";
                            continue;
                        }
                    }

                }
                item.externalcomment = extComment;

            }

            var vendors = ags.vendor_table.ToList();

            var vendorid = "";
            foreach (var item in vendorLoantrack)
            {
                foreach (var items in vendors)
                {
                    if (item.vendorid != null)
                    {
                        if (item.vendorid.ToString() == items.id.ToString())
                        {
                            string concatenated = items.companyname + " ( " + items.name + " ) ";
                            vendorid = concatenated;
                            break;
                        }
                        else if (items.id.ToString() != item.vendorid)
                        {
                            vendorid = "Not Updated";
                            continue;
                        }
                    }
                        
                }
                item.vendorid = vendorid;

            }


            return PartialView(loan_track);
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