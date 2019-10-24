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
using System.Web.Security;
using static agskeys.Controllers.AccountController;

namespace agskeys.Controllers.Clientele
{
   [AuthorizeUser]
    public class ClienteleController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: Clientele
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            List<customer_profile_table> custprf = ags.customer_profile_table.Where(x => x.id.ToString() == userid).ToList();
            ViewBag.customer_profile_table = custprf;


            // var assigne_id = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
            //var getCustomer = ags.customer_profile_table.ToList();
            var customer_loans = (from s in ags.loan_table
                                  join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                  where s.customerid == userid
                                  orderby sa.datex descending
                                  select s).Distinct().ToList();
            var requestamnt = customer_loans.Sum(t => Convert.ToDecimal(t.requestloanamt));
            var loanamnt = customer_loans.Sum(t => Convert.ToDecimal(t.loanamt));
            var disbursementamnt = customer_loans.Sum(t => Convert.ToDecimal(t.disbursementamt));
            var balance = customer_loans.Sum(s => (Convert.ToDecimal(s.loanamt)) - (Convert.ToDecimal(s.disbursementamt)));
            var interest = customer_loans.Sum(t => Convert.ToDecimal(t.rateofinterest));           
           

            ViewData["requestamnt"] = requestamnt;
            ViewData["loanamnt"] = loanamnt;
            ViewData["disbursementamnt"] = disbursementamnt;
            ViewData["balance"] = balance;

            //var sanction_percentage = (loanamnt * 100) / requestamnt;
            //decimal sanction_percentages = Math.Round(sanction_percentage, 2);
            //ViewData["sanction_percentage"] = sanction_percentages;
            //var disbursement_percentage = (disbursementamnt * 100) / loanamnt;
            //decimal disbursement_percentages = Math.Round(disbursement_percentage, 2);
            //ViewData["disbursement_percentage"] = disbursement_percentages;
            //var balance_percentage = (balance * 100) / loanamnt;           
            //decimal balance_percentages = Math.Round(balance_percentage, 2);            
            //ViewData["balance_percentage"] = balance_percentages;

            //ViewData["interest"] = interest / customer_loans.Count();

            if (requestamnt != 0)
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
           

            if (customer_loans.Count() != 0)
            {
                ViewData["interest"] = Math.Round((interest / customer_loans.Count()),2);
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

            //var getloantype = ags.loantype_table.ToList();
            //foreach (var item in customer_loans)
            //{
            //    foreach (var items in getloantype)
            //    {
            //        if (item.loantype == items.id.ToString())
            //        {
            //            item.loantype = items.loan_type;
            //            break;
            //        }
            //        else if (!ags.loan_table.Any(s => s.loantype.ToString() == items.id.ToString()))
            //        {
            //            item.loantype = "Not Updated";
            //        }
            //    }
            //}
            return View("~/Views/Clientele/Index.cshtml", customer_loans);
        }
        //{
        //    if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
        //    {
        //        return this.RedirectToAction("ClientLogout", "Account");
        //    }
        //    string username = Session["username"].ToString();
        //    string userid = Session["userid"].ToString();
        //    var customer_loans = (from loan_table in ags.loan_table where loan_table.customerid==userid orderby loan_table.id descending select loan_table).ToList();

        //    return View("~/Views/Clientele/Index.cshtml", customer_loans);
        //}


        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("ClientLogout", "Account");
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


            return PartialView("~/Views/Clientele/Track.cshtml", loan_track);
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
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

            if(user!= null)
            { 
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
            }
            else
            {
                user = null;
            }
            return PartialView("~/Views/Clientele/Details.cshtml", user);
        }





        public ActionResult UserProfile()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            else
            {
                var intId = Session["userid"].ToString();
                var Id = Convert.ToInt32(intId);
                var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
                return PartialView(user);
            }

        }

        public ActionResult EditProfile(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("ClientLogout", "Account");
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
            return PartialView(customer_profile_table);
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
                        return RedirectToAction("Index", "Clientele");
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
                            return RedirectToAction("Index", "Clientele");
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
                    var userCount = (from u in ags.admin_table where u.username == customer_profile_table.customerid select u).Count();
                    if (userCount == 0)
                    {
                        existing.customerid = customer_profile_table.customerid;
                    }
                    else
                    {
                        //existing.username = admin_table.username;
                        TempData["AE"] = "This user name is already exist";
                        //return PartialView("Edit", "SuperAdmin");
                        return RedirectToAction("Index", "Clientele");
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
                return RedirectToAction("Index", "Clientele");
            }
            return RedirectToAction("Index", "Clientele");
        }

        public ActionResult Password()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
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
                            return RedirectToAction("Index", "Clientele");
                        }
                    }
                    else
                    {
                        TempData["logAgain"] = "Oops.! Please Provide Valid Credentials.";
                        return RedirectToAction("ClientLogout", "Account");
                    }
                    ags.SaveChanges();



                    return RedirectToAction("Index", "Clientele");


                }
                else
                {
                    TempData["logAgain"] = "Oops.! Something Went Wrong.";
                    return RedirectToAction("ClientLogout", "Account");
                }
            }
            return RedirectToAction("Index", "Clientele");
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

                    if (form["customerName"] != null)
                    {
                        customerName = form["customerName"].ToString();
                    }
                    else
                    {
                        customerName = "Not Updated";
                    }
                    if (form["customerEmail"] != null)
                    {
                        customerEmail = form["customerEmail"].ToString();
                    }
                    else
                    {
                        customerEmail = "Not Updated";
                    }
                    if (form["customerPhone"] != null)
                    {
                        customerPhone = form["customerPhone"].ToString();
                    }
                    else
                    {
                        customerPhone = "Not Updated";
                    }
                    if (form["customerMessage"] != null)
                    {
                        customerMessage = form["customerMessage"].ToString();
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
                        return RedirectToAction("Index", "Clientele");
                    }
                    catch (Exception ex)
                    {
                        TempData["customerFailedMsg"] = ex.Message;
                        TempData["customerFailedMsg"] += "Oops.! Somethig Went Wrong.";
                        return RedirectToAction("Index", "Clientele");
                    }

                }
               
            }
            return RedirectToAction("Index", "AgskeysSite");

        }
    }

}