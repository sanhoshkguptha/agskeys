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
using static agskeys.Controllers.AccountController;

namespace agskeys.Controllers.MobileClientele
{
    [AuthorizeMobileUser]
    public class MobileClienteleController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: MobileClientele
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            List<customer_profile_table> custprf = ags.customer_profile_table.Where(x => x.id.ToString() == userid).ToList();
            ViewBag.customer_profile_table = custprf;
            
            var customer_loans = (from s in ags.loan_table
                                  join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                  where s.customerid == userid
                                  orderby sa.datex descending
                                  select s).Distinct().ToList();
            ViewData["loancount"] = customer_loans.Count();

            //if (!string.IsNullOrEmpty(customer_loans.Sum(t=>t.requestloanamt)))
            //{
            //    
            //}
            //else
            //{
            //    var requestamnt = 0;
            //}

            var requestamnt = customer_loans.Sum(t => Convert.ToDecimal(t.requestloanamt));
            var loanamnt = customer_loans.Sum(t => Convert.ToDecimal(t.loanamt));
            var disbursementamnt = customer_loans.Sum(t => Convert.ToDecimal(t.disbursementamt));
            var balance = customer_loans.Sum(s => (Convert.ToDecimal((s.loanamt)) - (Convert.ToDecimal(s.disbursementamt))));
            var interest = customer_loans.Sum(t => Convert.ToDecimal(t.rateofinterest));

           
            ViewData["requestamnt"] = requestamnt;
            ViewData["loanamnt"] = loanamnt;
            ViewData["disbursementamnt"] = disbursementamnt;
            ViewData["balance"] = balance;

            if(requestamnt != 0)
            {
                var sanction_percentage = (loanamnt * 100) / requestamnt;
                decimal sanction_percentages = Math.Round(sanction_percentage, 2);
                ViewData["sanction_percentage"] = sanction_percentages;
            }
            else
            {
                ViewData["sanction_percentage"] = 0;

            }
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
            
            if(customer_loans.Count() != 0)
            {
                ViewData["interest"] = Math.Round((interest / customer_loans.Count()), 2);                
            }
            else
            {
                ViewData["interest"] = 0;
            }

            
            var getbank = (from bank_table in ags.bank_table select bank_table).ToList();

            var bankid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getbank)
                {
                    if (item.bankid.ToString() == items.id.ToString())
                    {
                        bankid = items.bankname;
                        break;
                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        bankid = "Not Updated";
                        continue;
                    }
                }
                item.bankid = bankid;

            }
            return View("~/Views/MobileClientele/Index.cshtml");
        }


        public ActionResult Loan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            List<customer_profile_table> custprf = ags.customer_profile_table.Where(x => x.id.ToString() == userid).ToList();
            ViewBag.customer_profile_table = custprf;

            var customer_loans = (from s in ags.loan_table
                                  join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                  where s.customerid == userid
                                  orderby sa.datex descending
                                  select s).Distinct().ToList();
            var getbank = (from bank_table in ags.bank_table select bank_table).ToList();

            var bankid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getbank)
                {
                    if (item.bankid.ToString() == items.id.ToString())
                    {
                        bankid = items.bankname;
                        break;
                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        bankid = "Not Updated";
                        continue;
                    }
                }
                item.bankid = bankid;

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
            return View("~/Views/MobileClientele/Loan.cshtml",customer_loans);
        }
        public ActionResult profile()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/MobileClientele/profile.cshtml",user);
        }

        public ActionResult Enquiry()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/MobileClientele/Enquiry.cshtml", user);
        }
        [HttpPost]
        public ActionResult SendEnquiry(FormCollection form)
        {
            string userid = Session["userid"].ToString();
            if (userid != null)
            {
                int customerCount = ags.customer_profile_table.Where(x => x.id.ToString() == userid).Count();
                if (customerCount != 0)
                {
                    string customerName = "";
                    string customerEmail = "";
                    string customerPhone = "";
                    string customerMessage = "";

                    if (form["name"] != null)
                    {
                        customerName = form["name"].ToString();
                    }
                    else
                    {
                        customerName = "Not Updated";
                    }
                    if (form["email"] != null)
                    {
                        customerEmail = form["email"].ToString();
                    }
                    else
                    {
                        customerEmail = "Not Updated";
                    }
                    if (form["mobile"] != null)
                    {
                        customerPhone = form["mobile"].ToString();
                    }
                    else
                    {
                        customerPhone = "Not Updated";
                    }
                    if (form["message"] != null)
                    {
                        customerMessage = form["message"].ToString();
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
                    MyMailMessage.Subject = "AGSKEYS - Enquiry From Customer";
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
                        return RedirectToAction("Enquiry", "MobileClientele");
                    }
                    catch (Exception ex)
                    {
                        TempData["customerFailedMsg"] = ex.Message;
                        TempData["customerFailedMsg"] += "Oops.! Somethig Went Wrong.";
                        return RedirectToAction("Enquiry", "MobileClientele");
                    }

                }

            }
            return RedirectToAction("Index", "MobileClientle");

        }
        public ActionResult RequestLoan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;
            var model = new agskeys.Models.RequestLoan();
            model.name = user.name;
            model.phoneno = user.phoneno;
            model.email = user.email;

            return View("~/Views/MobileClientele/RequestLoan.cshtml", model);
        }
        public ActionResult Refer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileClientele/Refer.cshtml");
        }
        [HttpPost]
        public ActionResult Refer(FormCollection form)
        {
            string userid = Session["userid"].ToString();
            if (userid != null)
            {
                var customer = ags.customer_profile_table.Where(x => x.id.ToString() == userid).FirstOrDefault();
                int customerCount = customer.ToString().Count();
                if (customerCount != 0)
                {
                    var customerUserName = customer.name + "(" + customer.email + ")";
                    string customerName = "";
                    string customerEmail = "";
                    string customerPhone = "";

                    if (form["referName"] != null)
                    {
                        customerName = form["referName"].ToString();
                    }
                    else
                    {
                        customerName = "Not Updated";
                    }
                    if (form["referemail"] != null)
                    {
                        customerEmail = form["referemail"].ToString();
                    }
                    else
                    {
                        customerEmail = "Not Updated";
                    }
                    if (form["referNumber"] != null)
                    {
                        customerPhone = form["referNumber"].ToString();
                    }
                    else
                    {
                        customerPhone = "Not Updated";
                    }

                    //string CusEmail = "info@agskeys.com";
                    //string CusEmail = "info@agsfinancials.com";
                    string CusEmail = "santhosh@techvegas.in";
                    //////////////////////////////////

                    MailMessage MyMailMessage = new MailMessage();
                    MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                    MyMailMessage.To.Add(CusEmail);
                    MyMailMessage.Subject = "AGSKEYS - Referal From Customer";
                    MyMailMessage.IsBodyHtml = true;

                    MyMailMessage.Body = "<div style='font-family:Arial; font-size:16px; font-color:#d92027 '>Agskeys having New Referal Details ( " + customerUserName + " ).</div><br><table border='0' ><tr><td style='padding:25px;'>Name</td><td>" + customerName + "</td></tr><tr><td style='padding:25px;'>Email</td><td>" + customerEmail + "</td></tr><tr><td style='padding:25px;'>Phone</td><td>" + customerPhone + "</td></tr></table>";

                    SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                    SMTPServer.Port = 587;
                    SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                    SMTPServer.EnableSsl = true;
                    try
                    {
                        SMTPServer.Send(MyMailMessage);
                        TempData["customerSuccessMsg"] = "Your Referal Details Successfully send to AGSKEYS";
                        return RedirectToAction("Refer", "MobileClientele");
                    }
                    catch (Exception ex)
                    {
                        TempData["customerFailedMsg"] = ex.Message;
                        TempData["customerFailedMsg"] += "Oops.! Somethig Went Wrong.";
                        return RedirectToAction("Refer", "MobileClientele");
                    }

                }

            }
            return RedirectToAction("Index", "MobileClientele");

        }
        public ActionResult Support()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileClientele/Support.cshtml");
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
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

            return PartialView(user);
        }
        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<loan_table> loan = ags.loan_table.Where(x => x.id == Id).ToList();
            List<loan_track_table> employeeLoantrack = ags.loan_track_table.Where(x => x.loanid == Id.ToString()).ToList();
           

            var employee = ags.admin_table.ToList();
            loan_track loan_track = new loan_track();
            loan_track.loan_details = loan.ToList();
            loan_track.employee_track = employeeLoantrack.ToList().OrderBy(t => t.tracktime);            

            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
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
                            var employeeType = ags.emp_category_table.Where(x => x.emp_category_id == items.userrole).FirstOrDefault();
                            //string concatenated = items.name + " ( " + items.userrole + " ) ";
                            employeeid = employeeType.emp_category;
                            
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
            
            return PartialView(loan_track);
        }



        public ActionResult EditProfile(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            customer_profile_table customer_profile_table = ags.customer_profile_table.Find(Id);

            if (customer_profile_table == null)
            {
                return HttpNotFound();
            }
            return View(customer_profile_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(customer_profile_table customer_profile_table, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", ".jpeg"
                };
                customer_profile_table existing = ags.customer_profile_table.Find(customer_profile_table.id);
                var password = existing.password.ToString();
                var newPassword = customer_profile_table.password.ToString();


                if (existing.profileimg == null && customer_profile_table.ImageFile != null)
                {

                    string BigfileName = Path.GetFileNameWithoutExtension(customer_profile_table.ImageFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension1 = Path.GetExtension(customer_profile_table.ImageFile.FileName);
                    string extension = extension1.ToLower();
                    if (allowedExtensions.Contains(extension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                        customer_profile_table.profileimg = "~/customerImage/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/customerImage/"), fileName);
                        customer_profile_table.ImageFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                        return RedirectToAction("EditProfile", "MobileClientele");
                    }
                }


                else if (existing.profileimg != null && customer_profile_table.profileimg != null)
                {
                    if (customer_profile_table.ImageFile != null)
                    {
                        string path = Server.MapPath(existing.profileimg);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(customer_profile_table.ImageFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension1 = Path.GetExtension(customer_profile_table.ImageFile.FileName);
                        string extension = extension1.ToLower();
                        if (allowedExtensions.Contains(extension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                            customer_profile_table.profileimg = "~/customerImage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/customerImage/"), fileName);
                            customer_profile_table.ImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("EditProfile", "MobileClientele");
                        }

                    }
                    else
                    {
                        existing.profileimg = existing.profileimg;
                    }
                }
                else
                {
                    existing.profileimg = existing.profileimg;
                }
                existing.name = customer_profile_table.name;
                existing.email = customer_profile_table.email;
                existing.phoneno = customer_profile_table.phoneno;
                existing.weddingdate = customer_profile_table.weddingdate;
                existing.dob = customer_profile_table.dob;
                existing.address = customer_profile_table.address;
                existing.alterphoneno = customer_profile_table.alterphoneno;

                if (existing.customerid != customer_profile_table.customerid)
                {
                    var userCount = (from u in ags.customer_profile_table where u.customerid == customer_profile_table.customerid select u).Count();
                    if (userCount == 0)
                    {
                        existing.customerid = customer_profile_table.customerid;
                    }
                    else
                    {
                        //existing.username = admin_table.username;
                        TempData["Message"] = "This user name is already exist";
                        //return PartialView("Edit", "SuperAdmin");
                        return RedirectToAction("EditProfile", "MobileClientele");
                    }
                }


                existing.profileimg = customer_profile_table.profileimg;


               
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
                    existing.password = customer_profile_table.password;
                }
                else
                {
                    existing.password = PasswordStorage.CreateHash(customer_profile_table.password);
                }

                ags.SaveChanges();
                TempData["updateSuccess"] = "Your Profile Updated Successfully.!";
                return RedirectToAction("Profile", "MobileClientele");
            }
            return RedirectToAction("EditProfile", "MobileClientele");
        }



        public ActionResult Password()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
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
                    customer_profile_table existing = ags.customer_profile_table.Where(x => x.id.ToString() == userid).FirstOrDefault();

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
                            return RedirectToAction("Index", "MobileClientele");
                        }
                    }
                    else
                    {
                        TempData["logAgain"] = "Oops.! Please Provide Valid Credentials.";
                        return this.RedirectToAction("MobileLogout", "Account");
                    }
                    ags.SaveChanges();


                    TempData["PswdSuccess"] = "Your Password Reset Successfully";
                    return RedirectToAction("profile", "MobileClientele");


                }
                else
                {
                    TempData["logAgain"] = "Oops.! Something Went Wrong.";
                    return this.RedirectToAction("MobileLogout", "Account");
                }
            }
            return RedirectToAction("profile", "MobileClientele");
        }

    }
}