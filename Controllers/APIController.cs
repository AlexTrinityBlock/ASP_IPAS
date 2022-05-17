using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ASP_IPAS.Models;

namespace ASP_IPAS.Controllers
{
    public class APIController : Controller
    {
        /*------------------------------------*/

        //檢查是否有登入Session
        public bool checkLogin()
        {
            if ((string)(Session["IsLogin"]) == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*------------------------------------*/

        //測試用
        public ActionResult testHash(string q)
        {
            LoginModel loginModel = new LoginModel();
            return Content(loginModel.getHash(q));
        }

        public ActionResult testGetUserPasswordHash(string q)
        {
            MySQLModel mysqlModel = new MySQLModel();
            UserDataModel userDataModel = mysqlModel.getDataByUserName(q);
            return Content(userDataModel.password_hash);
        }

        /*------------------------------------*/
        //檢查是否登入成功
        public ActionResult isLogin()
        {
            string result = "";
            if (this.checkLogin())
            {
                result = "true";
            }
            else
            {
                result = "false";
            }

            var jsonData = new { data = result };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //登入
        public ActionResult login(string user, string password)
        {
            string result = "";
            LoginModel loginModel = new LoginModel();
            MySQLModel mysqlModel = new MySQLModel();
            UserDataModel userDataModel = mysqlModel.getDataByUserName(user);

            if (loginModel.checkUserPassword(user, password))
            {
                result = "true";
                Session["IsLogin"] = "true";
                Session["user"] = userDataModel.name;
            }
            else
            {
                result = "false";
            }

            var jsonData = new { data = result };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}