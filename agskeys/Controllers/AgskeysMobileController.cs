using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    public class AgskeysMobileController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: AgskeysMobile
        public ActionResult Index()
        {
            return View();
        }
    }
}