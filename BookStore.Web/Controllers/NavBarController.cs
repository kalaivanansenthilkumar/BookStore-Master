using BookStore.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Web.Controllers
{
    public class NavBarController : Controller
    {
        // GET: NavBar
        public ActionResult Index()
        {

            var data = new NavBarData();
            return PartialView("_Navbar", data.navbarItems().ToList());
        }
    }
}