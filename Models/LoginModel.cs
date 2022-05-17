using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace ASP_IPAS.Models
{
    public class LoginModel
    {
        //取得Hash值
        public string getHash(string inputString) {
            HashAlgorithm algorithm = SHA256.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString)))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        //檢查帳號密碼是否正確
        public bool checkUserPassword(string user,string password) { 
            MySQLModel mySQLModel = new MySQLModel();

            try
            {
                UserDataModel userDataModel = mySQLModel.getDataByUserName(user);
                string inputPasswordHash = getHash(password);

                if (userDataModel.password_hash == inputPasswordHash)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) {
                return false;
            }            
        }
    }
}