using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Customers()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Authors()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Edit()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Delete()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
