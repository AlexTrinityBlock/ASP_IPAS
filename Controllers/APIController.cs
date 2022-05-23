using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ASP_IPAS.Models;
using System.IO;

//帳號:Alice 密碼:123

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

        //取得排程任務資訊
        [HttpGet]
        public ActionResult getTaskInfo(string data)
        {
            MySQLModel mySQLModel = new MySQLModel();
            var jsonData = new { data = mySQLModel.getTaskData() };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //寫入排程任務資訊
        [HttpPost]
        public ActionResult postTaskInfo(string data)
        {
            MySQLModel mySQLModel = new MySQLModel();
            var taskDataObj = JsonConvert.DeserializeObject<TaskData>(data);
            mySQLModel.setTaskData(taskDataObj);
            return Content("");
        }

        //接收上傳的圖片檔案
        [HttpPost]
        public ActionResult file(HttpPostedFileBase file)
        {
            MySQLModel mysqlModel = new MySQLModel();
            byte[] imageBytes = null;

            //若檔案不為空時
            if (file != null && file.ContentLength > 0)
                //嘗試將收到的檔案取出，存入Bytes陣列
                try
                {
                    using (BinaryReader theReader = new BinaryReader(file.InputStream))
                    {
                        byte[] thePictureAsBytes = theReader.ReadBytes(file.ContentLength);
                        imageBytes = thePictureAsBytes;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                //檔案為空
                return Redirect("/Home/ImageStorage");
            }

            mysqlModel.saveFile(file.FileName, imageBytes);

            return Redirect("/Home/ImageStorage");

        }

        //取得圖片清單列表
        public ActionResult getFileList()
        {
            MySQLModel mySQLModel = new MySQLModel();
            List<FileData> fileList = mySQLModel.getFileList();
            var jsonData = JsonConvert.SerializeObject(fileList);
            var resultData = new { data = jsonData };
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        //檔案下載
        [HttpGet]
        public ActionResult downloadFile(string fileID)
        {
            MySQLModel mysqlModel = new MySQLModel();
            FileData fileData = mysqlModel.downloadFile(fileID);

            //回傳出檔案
            return File(fileData.fileData, "application/unknow", fileData.fileName);

        }
    }
}