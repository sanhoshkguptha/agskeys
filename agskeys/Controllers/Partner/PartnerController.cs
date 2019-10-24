using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.partner
{
    [Authorize]
    public class PartnerController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: Partner
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var customer_loans = (from loan_table in ags.loan_table where loan_table.partnerid == userid orderby loan_table.id descending select loan_table).ToList();


            var loanamnt = customer_loans.Sum(t => Convert.ToDecimal(t.loanamt));
            var disbursementamnt = customer_loans.Sum(t => Convert.ToDecimal(t.disbursementamt));
            var balance = customer_loans.Sum(s => (Convert.ToDecimal(s.loanamt)) - (Convert.ToDecimal(s.disbursementamt)));


            ViewData["loanamnt"] = loanamnt;
            ViewData["disbursementamnt"] = disbursementamnt;
            ViewData["balance"] = balance;

            if (loanamnt != 0)
            {
                var disbursement_percentage = (disbursementamnt * 100) / loanamnt;
                decimal disbursement_percentages = Math.Round(disbursement_percentage, 2);
                ViewData["disbursement_percentage"] = disbursement_percentages;

                var balance_percentage = (balance * 100) / loanamnt;
                decimal balance_percentages = Math.Round(balance_percentage, 2);
                ViewData["balance_percentage"] = balance_percentages;
            }
            else
            {
                ViewData["disbursement_percentage"] = 0;
                ViewData["balance_percentage"] = 0;
            }
           
            //var disbursement_percentage = (disbursementamnt * 100) / loanamnt;
            //decimal disbursement_percentages = Math.Round(disbursement_percentage, 2);
            //ViewData["disbursement_percentage"] = disbursement_percentages;
            //var balance_percentage = (balance * 100) / loanamnt;
            //decimal balance_percentages = Math.Round(balance_percentage, 2);
            //ViewData["balance_percentage"] = balance_percentages;

            var getCustomer = ags.customer_profile_table.ToList();
            SelectList customers = new SelectList(getCustomer, "id", "customerid");
            ViewBag.customerList = customers;

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
                item.employeetype = customerid;

            }
            List<vendor_table> custprf = ags.vendor_table.Where(x => x.id.ToString() == userid).ToList();
            ViewBag.vendor_table = custprf;

            if (userid != null)
            {
                int assigned_customer_loans = (from s in ags.loan_table
                                               join sa in ags.assigned_table on s.id.ToString() equals sa.loanid
                                               where sa.assign_vendor_id == userid
                                               orderby sa.datex
                                               select s).Distinct().OrderByDescending(t => t.id).Count();

                ViewData["assignedLoanCount"] = assigned_customer_loans;               
            }
            else
            {
                ViewData["assignedLoanCount"] = "0";               
            }

            return View("~/Views/Partner/Index.cshtml", customer_loans);
        }



        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;
            var model = new agskeys.Models.partner_customer();
            return PartialView("~/Views/Partner/Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(partner_customer obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;
            if (ModelState.IsValid)
            {
                string vendorName = Session["username"].ToString();
                // var customer = (from u in ags.customer_profile_table where u.customerid == obj.customerid select u).FirstOrDefault();
                var vendor = (from u in ags.vendor_table where u.username == vendorName select u).FirstOrDefault();


                if (vendor != null)
                {
                    customer_profile_table customerprofile = new customer_profile_table();
                    customerprofile.name = obj.name;
                    customerprofile.email = obj.email;
                    customerprofile.phoneno = obj.phoneno;
                    customerprofile.datex = DateTime.Now.ToString();
                    customerprofile.addedby = Session["username"].ToString();
                    ags.customer_profile_table.Add(customerprofile);
                    ags.SaveChanges();

                    int latestcustomerid = customerprofile.id;

                    customer_profile_table existing_Customer_Profile = ags.customer_profile_table.Find(customerprofile.id);
                    existing_Customer_Profile.customerid = latestcustomerid.ToString();
                    existing_Customer_Profile.password = PasswordStorage.CreateHash(existing_Customer_Profile.customerid);
                    ags.SaveChanges();

                    loan_table loan = new loan_table();
                    loan.customerid = latestcustomerid.ToString();
                    loan.partnerid = vendor.id.ToString();
                    loan.loantype = obj.loantype;
                    loan.requestloanamt = obj.requestloanamt;
                    loan.disbursementamt = "0";
                    loan.loanamt = "0";
                    loan.rateofinterest = "0";
                    loan.loanstatus = "Pending";
                    loan.datex = DateTime.Now.ToString();
                    loan.addedby = Session["username"].ToString();
                    ags.loan_table.Add(loan);
                    ags.SaveChanges();


                    //////////////////////////////////////
                    var superadminid = (from u in ags.admin_table where u.userrole == "super_admin" select u).FirstOrDefault();
                    string superemployeeid = superadminid.id.ToString();

                    int latestloanid = loan.id;

                    loan_track_table loan_track = new loan_track_table();
                    loan_track.loanid = latestloanid.ToString();
                    if (superemployeeid != null)
                    {
                        loan_track.employeeid = superemployeeid;
                        loan_track.tracktime = DateTime.Now.ToString();
                    }
                    if (obj.internalcomment != null)
                    {
                        loan_track.internalcomment = obj.internalcomment;
                        loan_track.externalcomment = "Not Updated";
                    }
                    loan_track.datex = DateTime.Now.ToString();
                    loan_track.addedby = Session["username"].ToString();
                    ags.loan_track_table.Add(loan_track);
                    ags.SaveChanges();


                    ///Assigned Employee

                    loan_track_table loan_track_employee = new loan_track_table();
                    if (Session["userid"] != null)
                    {
                        loan_track_employee.loanid = latestloanid.ToString();
                        loan_track_employee.employeeid = superemployeeid;
                        loan_track_employee.tracktime = DateTime.Now.ToString();
                        loan_track_employee.internalcomment = "Vendor Assigned";
                        loan_track_employee.externalcomment = "Vendor Assigned";

                        loan_track_employee.datex = DateTime.Now.ToString();
                        loan_track_employee.addedby = Session["username"].ToString();
                        ags.loan_track_table.Add(loan_track_employee);
                        ags.SaveChanges();
                    }


                    vendor_track_table vendor_track = new vendor_track_table();
                    if (Session["userid"] != null)
                    {
                        vendor_track.loanid = latestloanid.ToString();
                        vendor_track.vendorid = Session["userid"].ToString();
                        vendor_track.tracktime = DateTime.Now.ToString();
                        vendor_track.comment = "Assigned to Super Admin";
                        vendor_track.datex = DateTime.Now.ToString();
                        vendor_track.addedby = Session["username"].ToString();
                        ags.vendor_track_table.Add(vendor_track);
                        ags.SaveChanges();

                    }


                    //assigned table

                    assigned_table assigned = new assigned_table();
                    assigned.loanid = latestloanid.ToString();
                    if (superemployeeid != null)
                    {
                        assigned.assign_emp_id = superemployeeid;
                    }
                    if (Session["userid"] != null)
                    {
                        assigned.assign_vendor_id = Session["userid"].ToString();
                    }
                    assigned.datex = DateTime.Now.ToString();
                    assigned.addedby = Session["username"].ToString();
                    ags.assigned_table.Add(assigned);
                    ags.SaveChanges();

                    var userVendor = Session["username"].ToString();
                    var vendorname = ags.vendor_table.Where(x => x.username == userVendor).FirstOrDefault();
                    // Loan notification to Super admin and Admin
                    ags.notification_table.Add(new notification_table
                    {
                        notification = "New Loan has Created for " + obj.name + " By You.",
                        seenstatus = 1,
                        userid = vendorname.username,
                        addedby = Session["username"].ToString(),
                        datex = DateTime.Now.ToString(),
                    });
                    ags.notification_table.Add(new notification_table
                    {
                        notification = "New Customer " + obj.name + " has Created By" + vendor.companyname +"(Vendor)",
                        seenstatus = 1,
                        userid = "super_admin",
                        addedby = Session["username"].ToString(),
                        datex = DateTime.Now.ToString(),
                    });
                    ags.SaveChanges();
                    ////////////////////////////////////

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["AE"] = "This customer user name is already exist";
                    return RedirectToAction("Index");
                }
            }
            return View("~/Views/Partner/Create.cshtml", obj);
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("ClientLogout", "Account");
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

            return PartialView("~/Views/Partner/Details.cshtml", user);
        }






        public ActionResult UserProfile()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            else
            {
                var intId = Session["userid"].ToString();
                var Id = Convert.ToInt32(intId);
                var user = ags.vendor_table.Where(x => x.id == Id).FirstOrDefault();
                return PartialView(user);
            }

        }

        public ActionResult EditProfile(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            vendor_table vendor_table = ags.vendor_table.Find(Id);

            if (vendor_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(vendor_table);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(vendor_table vendor_table, FormCollection form)
        {
            if (ModelState.IsValid)
            {

                vendor_table existing = ags.vendor_table.Find(vendor_table.id);
                var password = existing.password.ToString();
                var newPassword = vendor_table.password.ToString();

               
    
                existing.name = vendor_table.name;
                existing.email = vendor_table.email;
                existing.phoneno = vendor_table.phoneno;
                existing.companyname = vendor_table.companyname;
                existing.address = vendor_table.address;


                if (existing.username != vendor_table.username)
                {
                    var userCount = (from u in ags.vendor_table where u.username == vendor_table.username select u).Count();
                    if (userCount == 0)
                    {
                        existing.username = vendor_table.username;
                    }
                    else
                    {
                        //existing.username = admin_table.username;
                        TempData["AE"] = "This user name is already exist";
                        //return PartialView("Edit", "SuperAdmin");
                        return RedirectToAction("Index", "Partner");
                    }
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
                    existing.password = vendor_table.password;
                }
                else
                {
                    existing.password = PasswordStorage.CreateHash(vendor_table.password);
                }

                ags.SaveChanges();
                return RedirectToAction("Index", "Partner");
            }
            return RedirectToAction("Index", "Partner");
        }

        public ActionResult Password()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("ClientLogout", "Account");
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
                if (userid != null)
                {
                    vendor_table existing = ags.vendor_table.Where(x => x.id.ToString() == userid).FirstOrDefault();

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
                            return RedirectToAction("Index", "Partner");
                        }
                    }
                    else
                    {
                        TempData["logAgain"] = "Oops.! Please Provide Valid Credentials.";
                        return RedirectToAction("ClientLogout", "Account");
                    }
                    ags.SaveChanges();



                    return RedirectToAction("Index", "Partner");


                }
                else
                {
                    TempData["logAgain"] = "Oops.! Something Went Wrong.";
                    return RedirectToAction("ClientLogout", "Account");
                }
            }
            return RedirectToAction("Index", "Partner");
        }

        [HttpPost]
        public ActionResult SendEnquiry(FormCollection form)
        {
            string userid = Session["userid"].ToString();
            if (userid != null)
            {
                int customerCount = ags.vendor_table.Where(x => x.id.ToString() == userid).Count();
                if (customerCount != 0)
                {
                    string customerName = "";
                    string customerEmail = "";
                    string customerPhone = "";
                    string customerMessage = "";

                    if (form["vendorName"] != null)
                    {
                        customerName = form["vendorName"].ToString();
                    }
                    else
                    {
                        customerName = "Not Updated";
                    }
                    if (form["vendorEmail"] != null)
                    {
                        customerEmail = form["vendorEmail"].ToString();
                    }
                    else
                    {
                        customerEmail = "Not Updated";
                    }
                    if (form["vendorPhone"] != null)
                    {
                        customerPhone = form["vendorPhone"].ToString();
                    }
                    else
                    {
                        customerPhone = "Not Updated";
                    }
                    if (form["vendorMessage"] != null)
                    {
                        customerMessage = form["vendorMessage"].ToString();
                    }
                    else
                    {
                        customerMessage = "Not Updated";
                    }

                    //string CusEmail = "info@agskeys.com";
                    //string CusEmail = "info@agsfinancials.com";
                    string CusEmail = "santhosh@techvegas.in";
                    //////////////////////////////////

                    MailMessage MyMailMessage = new MailMessage();
                    MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                    MyMailMessage.To.Add(CusEmail);
                    MyMailMessage.Subject = "AGSKEYS - Enquiry From Vendor";
                    MyMailMessage.IsBodyHtml = true;

                    MyMailMessage.Body = "<div style='font-family:Arial; font-size:16px; font-color:#d92027 '>Agskeys having New Customer Enquiry Details.</div><br><table border='0' ><tr><td style='padding:25px;'>Name</td><td>" + customerName + "</td></tr><tr><td style='padding:25px;'>Email</td><td>" + customerEmail + "</td></tr><tr><td style='padding:25px;'>Phone</td><td>" + customerPhone + "</td></tr><tr><td style='padding:25px;'>Message</td><td>" + customerMessage + "</td></tr></table>";

                    SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                    SMTPServer.Port = 587;
                    SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                    SMTPServer.EnableSsl = true;
                    try
                    {
                        SMTPServer.Send(MyMailMessage);
                        TempData["customerSuccessMsg"] = "Your New Enquiry Successfully send to AGSKEYS";
                        return RedirectToAction("Index", "Partner");
                    }
                    catch (Exception ex)
                    {
                        TempData["customerFailedMsg"] = ex.Message;
                        TempData["customerFailedMsg"] += "Oops.! Somethig Went Wrong.";
                        return RedirectToAction("Index", "Partner");
                    }

                }

            }
            return RedirectToAction("Index", "Partner");

        }
    }
}