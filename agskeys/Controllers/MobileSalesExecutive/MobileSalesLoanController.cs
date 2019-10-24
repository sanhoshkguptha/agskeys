using agskeys.Models;
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
    public class MobileSalesLoanController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult salesloan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
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
                                  orderby sa.datex descending
                                  select s).Distinct().ToList();
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

            var getBank = ags.bank_table.ToList();
            string bankname = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getBank)
                {
                    if (item.bankid == items.id.ToString())
                    {
                        bankname = items.bankname;
                        break;
                    }
                    else if (items.id.ToString() != item.bankid)
                    {
                        bankname = "Not Updated";
                        continue;
                    }
                }
                item.bankid = bankname;
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


            return View("~/Views/MobileSalesExecutive/SalesLoan/salesloan.cshtml", customer_loans);
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
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

            return PartialView("~/Views/MobileSalesExecutive/SalesLoan/Details.cshtml", user);
        }
        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var getCustomer = ags.customer_profile_table.ToList();
            SelectList customers = new SelectList(getCustomer, "id", "customerid");
            ViewBag.customerList = customers;

            var getBank = ags.bank_table.ToList();
            SelectList banks = new SelectList(getBank, "id", "bankname");
            ViewBag.bankList = banks;

            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;

            var empCategory = ags.emp_category_table.Where(x => x.emp_category_id != "admin" && x.emp_category_id != "super_admin").ToList();
            SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
            ViewBag.empCategories = empCategories;

            var employee = ags.admin_table.ToList();
            SelectList employees = new SelectList(employee, "id", "name");
            ViewBag.employees = employees;

            var ExtComment = ags.external_comment_table.ToList();
            SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
            ViewBag.commentList = commentlist;

            var prooflist = ags.proof_table.ToList();
            SelectList prooflists = new SelectList(prooflist, "id", "proofname");
            ViewBag.prooflists = prooflists;

            List<proof_table> prooftable = ags.proof_table.ToList();
            ViewBag.prooflists = prooftable;

            List<proof_customer_table> proofcus = ags.proof_customer_table.Where(x => x.customerid == userid).ToList();
            ViewBag.proofcus = proofcus;

            //List<emp_category_table> categoryList = ags.emp_category_table.ToList();
            //ViewBag.empCategories = new (caSelectListtegoryList, "emp_category_id", "emp_category");


            //List<loan_table> loan = ags.loan_table.Where(x => x.id == Id).ToList();
            //List<proof_table> proof_table = ags.proof_table.ToList();
            //List<proof_customer_table> proof_customer = ags.proof_customer_table.Where(x => x.customerid == userid).ToList();


            //Multiple_proofs_customer Multiple_pc = new Multiple_proofs_customer();
            //Multiple_pc.loan_table = loan.ToList();
            //Multiple_pc.proof_table = proof_table.ToList();
            //Multiple_pc.proof_customer_table = proof_customer.ToList();
            loan_table loan_table = ags.loan_table.Find(Id);
            if (loan_table == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/MobileSalesExecutive/SalesLoan/Edit.cshtml", loan_table);
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

                var getBank = ags.bank_table.ToList();
                SelectList banks = new SelectList(getBank, "id", "bankname");
                ViewBag.bankList = banks;

                var getloantype = ags.loantype_table.ToList();
                SelectList loantp = new SelectList(getloantype, "id", "loan_type");
                ViewBag.loantypeList = loantp;

                var empCategory = ags.emp_category_table.Where(x => x.emp_category_id != "admin" && x.emp_category_id != "super_admin").ToList();
                SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
                ViewBag.empCategories = empCategories;

                var employee = ags.admin_table.ToList();
                SelectList employees = new SelectList(employee, "id", "name");
                ViewBag.employees = employees;

                var ExtComment = ags.external_comment_table.ToList();
                SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
                ViewBag.commentList = commentlist;

                var allowedExtensions = new[] {
                    ".png", ".jpg", ".jpeg",".doc",".docx",".pdf"
                };
                loan_table existing = ags.loan_table.Find(loan_table.id);
                string partner = existing.partnerid;

                existing.customerid = loan_table.customerid;
                existing.bankid = loan_table.bankid;
                existing.loantype = loan_table.loantype;
                existing.requestloanamt = loan_table.requestloanamt;
                existing.followupdate = loan_table.followupdate;

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

                /////Proof
                //proof_customer_table proof_customer_table = new proof_customer_table();

                //if (loan_table.proofid != null)
                //{
                //    proof_customer_table.customerid = loan_table.customerid;
                //    proof_customer_table.proofid = loan_table.proofid;
                //    proof_customer_table.proofans = loan_table.proofans;

                //    List<proof_customer_table> proofcus = ags.proof_customer_table.ToList();
                //    proofcus.ForEach(x => ags.proof_customer_table.Add(x));

                //    proof_customer_table.datex = DateTime.Now.ToString();
                //    proof_customer_table.addedby = Session["username"].ToString();
                //    ags.proof_customer_table.Add(proof_customer_table);
                //    ags.SaveChanges();
                //}


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
                //if (loan_table.partnerid != null)
                //{
                //    existing_data.assign_vendor_id = loan_table.partnerid;
                //}
                //else
                //{
                //    existing_data.assign_vendor_id = Session["userid"].ToString();
                //}
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

                return RedirectToAction("salesloan");
            }
            return PartialView("~/Views/MobileSalesExecutive/SalesLoan/Edit.cshtml", loan_table);
        }

        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
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
                            string concatenated = items.name;
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
                            extComment = "External Comment Not Updated";
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
                            string concatenated = items.companyname;
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


            return PartialView("~/Views/MobileSalesExecutive/SalesLoan/Track.cshtml", loan_track);
        }
        public ActionResult Delete(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("MobileLogout", "Account");
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

            return PartialView("~/Views/MobileSalesExecutive/SalesLoan/Delete.cshtml", user);
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
            ags.loan_table.Remove(loan_table);
            ags.SaveChanges();

            return RedirectToAction("salesloan");
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