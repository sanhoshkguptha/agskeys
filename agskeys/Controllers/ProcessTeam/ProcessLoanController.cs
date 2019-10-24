using agskeys.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.ProcessTeam
{
    [Authorize]
    public class ProcessLoanController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult processloan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            List<assigned_table> assign = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
            ViewBag.assigned_loan = assign;


            // var assigne_id = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
            var getCustomer = ags.customer_profile_table.ToList();
            var customer_loans = (from s in ags.loan_table
                                  join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                  where sa.employeeid == userid
                                  orderby sa.datex
                                  select s).Distinct().OrderByDescending(t => t.id).ToList();
            // var customer_loans = (from loan_table in ags.loan_table orderby loan_table.id descending select loan_table).ToList();

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
                item.customerid = customerid;

            }

            //var getVendor = ags.vendor_table.ToList();
            //var partnerid = "";
            //foreach (var item in customer_loans)
            //{
            //    foreach (var items in getVendor)
            //    {
            //        if (item.partnerid == items.id.ToString())
            //        {
            //            partnerid = items.companyname;
            //            break;
            //        }
            //        else if (items.id.ToString() != item.partnerid)
            //        {
            //            partnerid = "Not Updated";
            //            continue;
            //        }

            //    }
            //    item.partnerid = partnerid;
            //}

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
            return View("~/Views/ProcessTeam/ProcessLoan/processloan.cshtml", customer_loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult processloan(FormCollection form)
        {
                if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
                {
                    return this.RedirectToAction("Logout", "Account");
                }
                string username = Session["username"].ToString();
                string userid = Session["userid"].ToString();
                List<assigned_table> assign = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
                ViewBag.assigned_loan = assign;


                // var assigne_id = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
                var getCustomer = ags.customer_profile_table.ToList();
                string SearchFor = "";
                var customer_loans = (from s in ags.loan_table
                                      join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                      where sa.employeeid == userid
                                      orderby sa.datex descending 
                                      select s).Distinct().ToList();
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
                                              where (s.disbursementamt == "0" && sa.employeeid == userid)
                                              orderby sa.datex descending
                                              select s).Distinct().ToList();

                    }
                    else if (SearchFor == "Partialydisbursed")
                    {
                        customer_loans = (from s in ags.loan_table
                                              join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                              where (s.disbursementamt != s.loanamt && s.disbursementamt != "0" && sa.employeeid == userid)
                                              orderby sa.datex descending
                                              select s).Distinct().ToList();
                        

                    }
                    else if (SearchFor == "fullydisbursed")
                    {                        
                        customer_loans = (from s in ags.loan_table
                                              join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                              where (s.disbursementamt == s.loanamt && s.requestloanamt != "0" && s.loanamt != "0" && sa.employeeid == userid)
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
                            customerid = items.customerid;
                            break;
                        }
                        else if (items.id.ToString() != item.customerid)
                        {
                            customerid = "Not Updated";
                            continue;
                        }
                    }
                    item.customerid = customerid;

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
                    item.partnerid = partnerid;
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
                return View("~/Views/ProcessTeam/ProcessLoan/processloan.cshtml", customer_loans);
                     

        }

        public JsonResult GetEmployeeList(string categoryId)
        {
            var username = Session["username"].ToString();
            ags.Configuration.ProxyCreationEnabled = false;
            List<admin_table> employees = ags.admin_table.Where(x => x.userrole.ToString() == categoryId && x.username != username).ToList();
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
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
            if(loan_count == 1)
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
            return PartialView("~/Views/ProcessTeam/ProcessLoan/Details.cshtml", user);
        }
        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
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
            SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
            ViewBag.empCategories = empCategories;

            var username = Session["username"].ToString();
            var employee = ags.admin_table.Where(x=>x.username != username).ToList();
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
            return PartialView("~/Views/ProcessTeam/ProcessLoan/Edit.cshtml", loan_table);
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
                SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
                ViewBag.empCategories = empCategories;

                var username = Session["username"].ToString();
                var employee = ags.admin_table.Where(x => x.username != username).ToList();
                SelectList employees = new SelectList(employee, "id", "name");
                ViewBag.employees = employees;


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
                        return RedirectToAction("processloan");
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
                            loan_table.sactionedcopy = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            loan_table.sactionedCopyFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("processloan");
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

                //ID copy file

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
                        return RedirectToAction("processloan");
                    }
                }


                else if (existing.idcopy != null && loan_table.idcopy != null)
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
                            loan_table.idcopy = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            loan_table.idCopyFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("processloan");
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
                existing.followupdate = loan_table.followupdate;
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

                if(loan_table.employee != null)
                {
                    admin_table admin_table_executive = ags.admin_table.Where(x => x.id.ToString() == loan_table.employee).FirstOrDefault();
                    string userrole_check = admin_table_executive.userrole;
                    if(userrole_check == "process_executive")
                    {
                        int latestloanid_check = loan_table.id;
                        process_executive process_executive = new process_executive();
                        if (latestloanid_check.ToString() != null)
                        {
                            process_executive.loanid = latestloanid_check.ToString();
                            process_executive.technical = "No";
                            process_executive.legal = "No";
                            process_executive.rcu = "No";
                            process_executive.comment = loan_table.internalcomment;
                            process_executive.datex = DateTime.Now.ToString();
                            process_executive.addedby = Session["username"].ToString();
                            ags.process_executive.Add(process_executive);
                            ags.SaveChanges();
                        }
                    }
                }

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
                assigned_table existing_data = ags.assigned_table.Where(x => x.loanid == loan_table.id.ToString()).FirstOrDefault();


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
                var employeename = ags.admin_table.Where(x => x.id.ToString() == loan_table.employee).FirstOrDefault();
                if (employeename != null)
                {
                    ags.notification_table.Add(new notification_table
                    {
                        notification = "Loan " + loan_table.customerid + " Assigned" + " to " + employeename.name,
                        seenstatus = 1,
                        userid = "super_admin",
                        addedby = Session["username"].ToString(),
                        datex = DateTime.Now.ToString(),
                    });
                }
                ags.notification_table.Add(new notification_table
                {
                    notification = "Loan (" + loan_table.customerid + ") Assigned" + " to you",
                    seenstatus = 1,
                    userid = loan_table.employee,
                    addedby = Session["username"].ToString(),
                    datex = DateTime.Now.ToString(),
                });
                ags.SaveChanges();

                return RedirectToAction("processloan");
            }
            return PartialView("~/Views/ProcessTeam/ProcessLoan/Edit.cshtml", loan_table);
        }

        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
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


            return PartialView("~/Views/ProcessTeam/ProcessLoan/Track.cshtml", loan_track);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            string username = Session["username"].ToString();
            ////var customersl = (from s in ags.customer_profile_table
            ////                 join sa in ags.loan_table on s.customerid equals sa.customerid into rd
            ////                 from rt in rd.DefaultIfEmpty()
            ////                 join sb in ags.assigned_table on rt.id.ToString() equals sb.loanid into rb
            ////                 from rc in rb.DefaultIfEmpty()
            ////                 where s.customerid == username || s.addedby == username
            ////                 orderby rc.datex
            ////                 select s).Distinct().OrderByDescending(t => t.id).ToList();
          
           // var customer = (from cus in ags.customer_profile_table join loans in ags.loan_table on cus.id.ToString() equals loans.customerid into total from totals in total.DefaultIfEmpty() join assign in ags.assigned_table on totals.id.ToString() equals assign.loanid into rb from rc in rb.DefaultIfEmpty() where cus.customerid == username orderby rc.datex select cus).Distinct().OrderByDescending(t => t.id).ToList();

            var getCustomer = ags.customer_profile_table.Where(x => x.addedby == username).ToList();
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


            var model = new agskeys.Models.loan_table();
            return PartialView("~/Views/ProcessTeam/ProcessLoan/Create.cshtml", model);
        }


    


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(loan_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                string username = Session["username"].ToString();
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
                if (obj.partnerid != null)
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
                ags.SaveChanges();
                return RedirectToAction("Loan");

            }
            else
            {
                TempData["AE"] = "Something went wrong";
                return RedirectToAction("Loan");
            }
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