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
        //主頁
        public ActionResult Index()
        {
            if ((string)(Session["IsLogin"]) == "true")
            {
                return View();
            }
            else
            {
                return Redirect("/Home/Login");
            }
        }

        //登入
        public ActionResult Login()
        {
            return View();
        }

    }
}