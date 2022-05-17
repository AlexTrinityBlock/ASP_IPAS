using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASP_IPAS.Models;

namespace ASP_IPAS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            MySQLModel model = new MySQLModel();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}