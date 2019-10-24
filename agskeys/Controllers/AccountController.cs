using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace agskeys.Controllers
{
    public class AccountController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: Account //
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            var getEmployeeCategoty = ags.emp_category_table.Where(x => x.status == "publish" && x.emp_category_id != "partner" && x.emp_category_id != "clientele").ToList();
            SelectList list = new SelectList(getEmployeeCategoty, "emp_category_id", "emp_category");
            ViewBag.categoryList = list;
            // ags.admin_table = new admin_table();
            return View();
        }
        public ActionResult MobileLogin()
        {

            return RedirectToAction("Index", "AgskeysMobile");
        }
        [HttpPost]
        public ActionResult MobileLogin(FormCollection form, vendor_table obj)
        {
            if (form["userlevel"].ToString() == "")
            {
                TempData["userRoleMissing"] = "please select userrole";
                return RedirectToAction("Index", "AgskeysMobile");
            }
            else if (form["userlevel"].ToString() == "partner")
            { 
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var vndr = (from u in ags.vendor_table where u.username == userName select u).FirstOrDefault();
                if (vndr == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }
                else if (vndr != null)
                {
                    var model = ags.vendor_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = vndr.id.ToString();
                        Session["username"] = vndr.username.ToString();
                        Session["userlevel"] = "partner";
                        FormsAuthentication.SetAuthCookie(vndr.username, false);
                        return RedirectToAction("Index", "MobileVendor");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysMobile");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }

            }
            else if (form["userlevel"].ToString() == "clientele")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var customer = (from u in ags.customer_profile_table where u.customerid == userName select u).FirstOrDefault();
                if (customer == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }
                else if (customer != null)
                {
                    var model = ags.customer_profile_table.Where(x => x.customerid == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = customer.id.ToString();
                        Session["username"] = customer.customerid.ToString();
                        Session["userlevel"] = "clientele";
                        FormsAuthentication.SetAuthCookie(customer.customerid, false);
                        return RedirectToAction("Index", "MobileClientele");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysMobile");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }

            }
            else if (form["userlevel"].ToString() == "sales_executive")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var sales_executive = (from u in ags.admin_table where u.username == userName && u.isActive == true select u).FirstOrDefault();
                if (sales_executive == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }
                else if (sales_executive != null)
                {
                    var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == userlevel && x.status == "publish").SingleOrDefault();


                    if (result)
                    {
                        Session["userid"] = sales_executive.id.ToString();
                        Session["username"] = sales_executive.username.ToString();
                        FormsAuthentication.SetAuthCookie(sales_executive.username, false);                              
                        if (emp.emp_category_id == "sales_executive" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = form["userlevel"].ToString();
                            return RedirectToAction("salesloan", "MobileSalesLoan");
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Index", "AgskeysMobile");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysMobile");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }

            }

            else if (form["userlevel"].ToString() == "manager")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var manager = (from u in ags.admin_table where u.username == userName && u.isActive == true select u).FirstOrDefault();
                if (manager == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }
                else if (manager != null)
                {
                    var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == userlevel && x.status == "publish").SingleOrDefault();


                    if (result)
                    {
                        Session["userid"] = manager.id.ToString();
                        Session["username"] = manager.username.ToString();
                        FormsAuthentication.SetAuthCookie(manager.username, false);
                        if (emp.emp_category_id == "manager" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = form["userlevel"].ToString();
                            return RedirectToAction("managerloan", "MobileManagerLoan");
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Index", "AgskeysMobile");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysMobile");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }

            }
            return View();

        }



        [HttpPost]
        public ActionResult ClientLogin(FormCollection form, vendor_table obj)
        {
            if (form["userlevel"].ToString() == "")
            {
                TempData["Message"] = "please select userrole";
                return RedirectToAction("Index", "AgskeysSite");
            }
            else if (form["userlevel"].ToString() == "partner")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var vndr = (from u in ags.vendor_table where u.username == userName select u).FirstOrDefault();
                if (vndr == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                else if (vndr != null)
                {
                    var model = ags.vendor_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = vndr.id.ToString();
                        Session["username"] = vndr.username.ToString();
                        Session["userlevel"] = "partner";
                        FormsAuthentication.SetAuthCookie(vndr.username, false);
                        return RedirectToAction("Index", "Partner");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysSite");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            else if (form["userlevel"].ToString() == "clientele")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var customer = (from u in ags.customer_profile_table where u.customerid == userName select u).FirstOrDefault();
                if (customer == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                else if (customer != null)
                {
                    var model = ags.customer_profile_table.Where(x => x.customerid == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = customer.id.ToString();
                        Session["username"] = customer.customerid.ToString();
                        Session["userlevel"] = "clientele";
                        FormsAuthentication.SetAuthCookie(customer.customerid, false);
                        return RedirectToAction("Index", "clientele");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysSite");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            else if (form["userlevel"].ToString() == "sales_executive")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var sales_executive = (from u in ags.admin_table where u.username == userName && u.isActive == true select u).FirstOrDefault();
                if (sales_executive == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                else if (sales_executive != null)
                {
                    var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == userlevel && x.status == "publish").SingleOrDefault();


                    if (result)
                    {
                        Session["userid"] = sales_executive.id.ToString();
                        Session["username"] = sales_executive.username.ToString();
                        FormsAuthentication.SetAuthCookie(sales_executive.username, false);
                        if (emp.emp_category_id == "sales_executive" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = form["userlevel"].ToString();
                            return RedirectToAction("Index", "SalesExecutive");
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Index", "AgskeysSite");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysSite");
                    }
                }

                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            else if (form["userlevel"].ToString() == "manager")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var manager = (from u in ags.admin_table where u.username == userName && u.isActive == true select u).FirstOrDefault();
                if (manager == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                else if (manager != null)
                {
                    var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == userlevel && x.status == "publish").SingleOrDefault();


                    if (result)
                    {
                        Session["userid"] = manager.id.ToString();
                        Session["username"] = manager.username.ToString();
                        FormsAuthentication.SetAuthCookie(manager.username, false);
                        if (emp.emp_category_id == "manager" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = form["userlevel"].ToString();
                            return RedirectToAction("Index", "Manager");
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Index", "AgskeysSite");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysSite");
                    }
                }

                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            return View();

        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        [AuthorizeUser]
        public ActionResult ClientLogout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "AgskeysSite");
        }

        [AuthorizeMobileUser]
        public ActionResult MobileLogout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "AgskeysMobile");
        }



        [HttpPost]
        public ActionResult Login(FormCollection form, admin_table obj)
        {
            string userName = form["userName"].ToString();
            string password = form["password"].ToString();
            if (obj.userrole == null)
            {
                TempData["Message"] = "please select userrole";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string userlevel = obj.userrole.ToString();
                if (obj.userrole == "partner")
                {
                    var vndr = (from u in ags.vendor_table where u.username == userName select u).FirstOrDefault();
                    if (vndr == null)
                    {
                        TempData["Message"] = "username or password is wrong";
                        return RedirectToAction("Login", "Account");
                        // return View();
                    }
                    else if (vndr != null)
                    {
                        var model = ags.vendor_table.Where(x => x.username == userName).SingleOrDefault();
                        bool result = PasswordStorage.VerifyPassword(password, model.password);

                        string usrrole = obj.userrole.ToString();
                        var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == usrrole && x.status == "publish").SingleOrDefault();
                        if (result)
                        {
                            Session["userid"] = vndr.id.ToString();
                            Session["username"] = vndr.username.ToString();
                            FormsAuthentication.SetAuthCookie(vndr.username, false);
                            if (emp.emp_category_id == "partner")
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "Partner");
                            }
                            else
                            {
                                TempData["Message"] = "Enter the valid user credentials";
                                return RedirectToAction("Login", "Account");
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "username or password is wrong";
                        return RedirectToAction("Login", "Account");
                    }

                }
                else if (obj.userrole == "clientele")
                {
                    var clientele = (from u in ags.customer_profile_table where u.customerid == userName select u).FirstOrDefault();
                    if (clientele == null)
                    {
                        TempData["Message"] = "username or password is wrong";
                        return RedirectToAction("Login", "Account");
                        // return View();
                    }
                    else if (clientele != null)
                    {
                        var model = ags.customer_profile_table.Where(x => x.customerid == userName).SingleOrDefault();
                        bool result = PasswordStorage.VerifyPassword(password, model.password);

                        string usrrole = obj.userrole.ToString();
                        var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == usrrole && x.status == "publish").SingleOrDefault();
                        if (result)
                        {
                            Session["userid"] = clientele.id.ToString();
                            Session["username"] = clientele.customerid.ToString();
                            FormsAuthentication.SetAuthCookie(clientele.customerid, false);
                            if (emp.emp_category_id == "clientele")
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "Clientele");
                            }
                            else
                            {
                                TempData["Message"] = "Enter the valid user credentials";
                                return RedirectToAction("Login", "Account");
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "username or password is wrong";
                        return RedirectToAction("Login", "Account");
                    }

                }


                else
                {

                    var user = (from u in ags.admin_table where u.username == userName && u.userrole == userlevel && u.isActive == true select u).FirstOrDefault();
                    if (user == null)
                    {
                        TempData["Message"] = "username or password is wrong";
                        return RedirectToAction("Login", "Account");
                        // return View();
                    }
                    else if (user != null)
                    {
                        var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                        bool result = PasswordStorage.VerifyPassword(password, model.password);

                        string usrrole = obj.userrole.ToString();
                        var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == usrrole && x.status == "publish").SingleOrDefault();
                        if (result)
                        {
                            Session["userid"] = user.id.ToString();
                            Session["username"] = user.username.ToString();
                            FormsAuthentication.SetAuthCookie(user.username, false);
                            if (emp.emp_category_id == "super_admin" && emp.emp_category_id == model.userrole)
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "SuperAdmin");
                            }
                            else if (emp.emp_category_id == "admin" && emp.emp_category_id == model.userrole)
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (emp.emp_category_id == "sales_executive" && emp.emp_category_id == model.userrole)
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "SalesExecutive");
                            }
                            else if (emp.emp_category_id == "tele_marketing" && emp.emp_category_id == model.userrole)
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "TeleMarketing");
                            }
                            else if (emp.emp_category_id == "process_team" && emp.emp_category_id == model.userrole)
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index","ProcessTeam");
                            }
                            else if (emp.emp_category_id == "process_executive" && emp.emp_category_id == model.userrole)
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "ProcessExecutive");
                            }
                            else if (emp.emp_category_id == "manager" && emp.emp_category_id == model.userrole)
                            {
                                Session["userlevel"] = obj.userrole.ToString();
                                return RedirectToAction("Index", "Manager");
                            }
                            else
                            {
                                TempData["Message"] = "Enter the valid user credentials";
                                return RedirectToAction("Login", "Account");
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "username or password is wrong";
                        return RedirectToAction("Login", "Account");
                    }

                }
            }
            
           

        }


        public class AuthorizeUserAttribute : AuthorizeAttribute
        {

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                var username = filterContext.HttpContext.User.Identity.Name;
                if (username != "")
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Account", action = "ClientLogin" }));
                }
            }
        }



        public class AuthorizeMobileUserAttribute : AuthorizeAttribute
        {

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                var username = filterContext.HttpContext.User.Identity.Name;
                if (username != "")
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Account", action = "MobileLogin" }));
                }
            }
        }
        ////site////
        public ActionResult agskey()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(FormCollection form)
        {
            string userName = form["userName"].ToString();
            if(userName != null)
            {
                int EmployeeCount = ags.admin_table.Where(x => x.username == userName).Count();                
                if(EmployeeCount != 0)
                {
                    admin_table employees = ags.admin_table.Where(x => x.username == userName).FirstOrDefault();
                    
                    string AutoGenPwd = Membership.GeneratePassword(12, 1);
                    string EmpEmail = employees.email;
                    if(EmpEmail != null)
                    {
                        string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                        employees.password = EncryptPassword;

                        //////////////////////////////////

                        MailMessage MyMailMessage = new MailMessage();
                        MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                        MyMailMessage.To.Add(EmpEmail);
                        MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                        MyMailMessage.IsBodyHtml = true;

                        MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                        SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                        SMTPServer.Port = 587;
                        SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                        SMTPServer.EnableSsl = true;
                        try
                        {
                            SMTPServer.Send(MyMailMessage);
                            ags.SaveChanges();
                            TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                            return RedirectToAction("ForgotPassword", "Account");
                        }
                        catch (Exception ex)
                        {
                            TempData["mail"] = ex.Message;
                            TempData["mail"] = "Oops.! Somethig Went Wrong.";
                            return RedirectToAction("ForgotPassword", "Account");
                        }
                    }
                    else
                    {
                        TempData["email"] = "Oops.! Somethig Went Wrong.";
                    }
                    

                    //////////////////////////////////
                }
                else
                {
                    TempData["NotExst"] = "Oops.! Something Went Wrong.";
                }
            }
            return View();
        }
        /// froount end forgot password section
        public ActionResult ForgotPasswordfront()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPasswordfront(FormCollection form)
        {
            string userlevel = form["userlevel"].ToString();
            if(userlevel == "sales_executive")
            {
                string userName = form["userName"].ToString();
                if (userName != null)
                {
                    int EmployeeCount = ags.admin_table.Where(x => x.username == userName).Count();
                    if (EmployeeCount != 0)
                    {
                        admin_table employees = ags.admin_table.Where(x => x.username == userName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string EmpEmail = employees.email;
                        if (EmpEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            employees.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(EmpEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }
                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return RedirectToAction("Index", "AgskeysSite");
            }
            else if (userlevel == "manager")
            {
                string userName = form["userName"].ToString();
                if (userName != null)
                {
                    int EmployeeCount = ags.admin_table.Where(x => x.username == userName).Count();
                    if (EmployeeCount != 0)
                    {
                        admin_table employees = ags.admin_table.Where(x => x.username == userName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string EmpEmail = employees.email;
                        if (EmpEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            employees.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(EmpEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }
                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return RedirectToAction("Index", "AgskeysSite");
            }
            else if (userlevel == "partner")
            {
                string vendorName = form["userName"].ToString();
                if (vendorName != null)
                {
                    int vendorCount = ags.vendor_table.Where(x => x.username == vendorName).Count();
                    if (vendorCount != 0)
                    {
                        vendor_table vendors = ags.vendor_table.Where(x => x.username == vendorName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string EmpEmail = vendors.email;
                        if (EmpEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            vendors.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(EmpEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }
                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return RedirectToAction("Index", "AgskeysSite");
            }
            else if (userlevel == "clientele")
            {
                string clientName = form["userName"].ToString();
                if (clientName != null)
                {
                    int vendorCount = ags.customer_profile_table.Where(x => x.customerid == clientName).Count();
                    if (vendorCount != 0)
                    {
                        customer_profile_table customers = ags.customer_profile_table.Where(x => x.customerid == clientName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string CusEmail = customers.email;
                        if (CusEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            customers.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(CusEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("Index", "AgskeysSite");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }


                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return RedirectToAction("Index", "AgskeysSite");
            }
            return RedirectToAction("Index", "AgskeysSite");
        }

        /// mobile end forgot password section
        public ActionResult ForgotPasswordMobile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPasswordMobile(FormCollection form)
        {
            string userlevel = form["userlevel"].ToString();
            if (userlevel == "sales_executive")
            {
                string userName = form["userName"].ToString();
                if (userName != null)
                {
                    int EmployeeCount = ags.admin_table.Where(x => x.username == userName).Count();
                    if (EmployeeCount != 0)
                    {
                        admin_table employees = ags.admin_table.Where(x => x.username == userName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string EmpEmail = employees.email;
                        if (EmpEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            employees.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(EmpEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }
                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return View();
            }
            else if (userlevel == "manager")
            {

                string userName = form["userName"].ToString();
                if (userName != null)
                {
                    int EmployeeCount = ags.admin_table.Where(x => x.username == userName).Count();
                    if (EmployeeCount != 0)
                    {
                        admin_table employees = ags.admin_table.Where(x => x.username == userName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string EmpEmail = employees.email;
                        if (EmpEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            employees.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(EmpEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }
                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return View();

            }
            else if (userlevel == "partner")
            {
                string vendorName = form["userName"].ToString();
                if (vendorName != null)
                {
                    int vendorCount = ags.vendor_table.Where(x => x.username == vendorName).Count();
                    if (vendorCount != 0)
                    {
                        vendor_table vendors = ags.vendor_table.Where(x => x.username == vendorName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string EmpEmail = vendors.email;
                        if (EmpEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            vendors.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(EmpEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }
                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return View();
            }
            else if (userlevel == "clientele")
            {
                string clientName = form["userName"].ToString();
                if (clientName != null)
                {
                    int vendorCount = ags.customer_profile_table.Where(x => x.customerid == clientName).Count();
                    if (vendorCount != 0)
                    {
                        customer_profile_table customers = ags.customer_profile_table.Where(x => x.customerid == clientName).FirstOrDefault();

                        string AutoGenPwd = Membership.GeneratePassword(12, 1);
                        string CusEmail = customers.email;
                        if (CusEmail != null)
                        {
                            string EncryptPassword = PasswordStorage.CreateHash(AutoGenPwd);
                            customers.password = EncryptPassword;

                            //////////////////////////////////

                            MailMessage MyMailMessage = new MailMessage();
                            MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                            MyMailMessage.To.Add(CusEmail);
                            MyMailMessage.Subject = "AGSKEYS - Auto Generated Password";
                            MyMailMessage.IsBodyHtml = true;

                            MyMailMessage.Body = "<div style='font - family: Arial; font - size: 12px; '>You have requested a password reset, please use this password to open your account.</div><br><table border='0' ><tr><td style='padding:25px;'>Your New Password</td><td style='padding:25px;'>" + AutoGenPwd + "</table></tr></td>";

                            SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                            SMTPServer.Port = 587;
                            SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                            SMTPServer.EnableSsl = true;
                            try
                            {
                                SMTPServer.Send(MyMailMessage);
                                ags.SaveChanges();
                                TempData["mail"] = "New Password Successfully Send to Your Registered Email";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                            catch (Exception ex)
                            {
                                TempData["mail"] = ex.Message;
                                TempData["mail"] = "Oops.! Somethig Went Wrong.";
                                return RedirectToAction("ForgotPasswordMobile", "Account");
                            }
                        }
                        else
                        {
                            TempData["email"] = "Oops.! Somethig Went Wrong.";
                        }


                        //////////////////////////////////
                    }
                    else
                    {
                        TempData["NotExst"] = "Oops.! Something Went Wrong.";
                    }
                }
                return View();
            }
            return View();
        }
        public ActionResult privacypolicy()
        {
            return View();
        }
        public JsonResult GetNotificationList()
        {
            ags.Configuration.ProxyCreationEnabled = false;
            List<notification_table> notifications = ags.notification_table.Where(x => x.userid.ToString() == "super_admin" && x.seenstatus == 1).ToList();
            return Json(notifications, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Subscription(FormCollection form)
        {            
            if (form["subscriptionEmail"] != null && form["subscriptionEmail"] != "")
            {

                string EmailId = "";
                EmailId = form["subscriptionEmail"].ToString();

                //string CusEmail = "info@agskeys.com";
                string CusEmail = "info@agsfinancials.com";
                //string CusEmail = "santhosh@techvegas.in";
                //////////////////////////////////

                MailMessage MyMailMessage = new MailMessage();
                MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                MyMailMessage.To.Add(CusEmail);
                MyMailMessage.Subject = "AGSKEYS - Subcription Email";
                MyMailMessage.IsBodyHtml = true;

                MyMailMessage.Body = "<div style='font-family:Arial; font-size:16px; font-color:#d92027 '>Agskeys having New Subscriber.</div><br><table border='0' ><tr><td style='padding:25px;'>Subcriber Email Id</td><td>" + EmailId + "</td></tr></table>";

                SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                SMTPServer.Port = 587;
                SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                SMTPServer.EnableSsl = true;
                try
                {
                    SMTPServer.Send(MyMailMessage);
                    TempData["customerSuccessMsg"] =  "Your Successfully Subscribed to AGSKEYS";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                catch (Exception ex)
                {
                    TempData["customerSuccessMsg"] = ex.Message;
                    TempData["customerSuccessMsg"] += "Oops.! Somethig Went Wrong.";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            return RedirectToAction("Index", "AgskeysSite");

        }

        [HttpPost]
        public ActionResult SendEnquiry(FormCollection form)
        {
            if (form["name"] != null && form["email"] != null && form["phone"] != null && form["message"] != null)
            {

                string name = "";
                string email = "";
                string phone = "";
                string message = "";
                name = form["name"].ToString();
                email = form["email"].ToString();
                phone = form["phone"].ToString();
                message = form["message"].ToString();

                //string CusEmail = "info@agskeys.com";
                string CusEmail = "info@agsfinancials.com";
                //string CusEmail = "santhosh @techvegas.in";
                //////////////////////////////////

                MailMessage MyMailMessage = new MailMessage();
                MyMailMessage.From = new MailAddress("auxinstore@gmail.com");
                MyMailMessage.To.Add(CusEmail);
                MyMailMessage.Subject = "AGSKEYS - Contact Enquiry Form";
                MyMailMessage.IsBodyHtml = true;

                MyMailMessage.Body = "<div style='font-family:Arial; font-size:16px; font-color:#d92027 '>Agskeys having New Enquiry from AGSKEYS.COM Website.</div><br><table border='0' ><tr><td style='padding:25px;'>Name</td><td>" + name + "</td></tr><tr><td style='padding:25px;'>Email</td><td>" + email + "</td></tr><tr><td style='padding:25px;'>Phone Number</td><td>" + phone + "</td></tr><tr><td style='padding:25px;'>Message</td><td>" + message + "</td></tr></table>";

                SmtpClient SMTPServer = new SmtpClient("smtp.gmail.com");
                SMTPServer.Port = 587;
                SMTPServer.Credentials = new System.Net.NetworkCredential("auxinstore@gmail.com", "auxin12345");
                SMTPServer.EnableSsl = true;
                try
                {
                    SMTPServer.Send(MyMailMessage);
                    TempData["EnquirySuccessMsg"] = "Your Successfully Send Enquiry Message to AGSKEYS";
                    return RedirectToAction("contact_us", "AgskeysSite");
                    //return View();
                }
                catch (Exception ex)
                {
                    TempData["EnquirySuccessMsg"] = ex.Message;
                    TempData["EnquirySuccessMsg"] += "Oops.! Somethig Went Wrong.";
                    return RedirectToAction("contact_us", "AgskeysSite");
                    //return View();
                }

            }
            return RedirectToAction("contact_us", "AgskeysSite");

        }
        /////corrected all/////////////////////////////////////////

    }
}