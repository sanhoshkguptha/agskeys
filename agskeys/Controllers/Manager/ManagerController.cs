using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.Manager
{
    [Authorize]
    public class ManagerController : Controller
    {
        // GET: Manager
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Index()
        {

            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var name = Session["username"].ToString();
            var userid = Session["userid"].ToString();
            var photo = ags.admin_table.Where(t => t.userrole == "manager" && t.username == name).ToList();
            ViewData["photo"] = photo.FirstOrDefault().photo;
            if (userid != null)
            {
                int assigned_customer_loans = (from s in ags.loan_table
                                               join sa in ags.assigned_table on s.id.ToString() equals sa.loanid
                                               where sa.assign_emp_id == userid
                                               orderby sa.datex
                                               select s).Distinct().OrderByDescending(t => t.id).Count();

                ViewData["assignedLoanCount"] = assigned_customer_loans;
                int customer_loans = (from s in ags.loan_table
                                      join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                      where sa.employeeid == userid
                                      orderby sa.datex descending
                                      select s).Distinct().Count();
                ViewData["customer_loans"] = customer_loans;
            }
            else
            {
                ViewData["assignedLoanCount"] = "0";
                ViewData["customer_loans"] = "0";
            }
            return View("~/Views/Manager/Manager/Index.cshtml");
        }



        public ActionResult Customer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
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

            return PartialView("~/Views/Manager/Manager/Customer.cshtml", customers);
        }
        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/Manager/Manager/Details.cshtml", user);
        }
    }
}