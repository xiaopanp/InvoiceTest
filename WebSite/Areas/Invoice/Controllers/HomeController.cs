using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Areas.Invoice.Controllers
{
    public class HomeController : Controller
    {
        // GET: Invoice/Home
        public ActionResult Index()
        {
            ViewBag.Title = "Invoice ";
            ViewBag.Message = "Invoice description page.";
            return View();
        }
    }
}