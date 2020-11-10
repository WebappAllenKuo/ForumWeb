using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace ForumWebsite.Models.Other
{
    public class Method_Cs : System.Web.UI.Page
    {
        public static ForumWebsiteDBEntities EntityConnention_Md()
        {
            ForumWebsiteDBEntities db = new ForumWebsiteDBEntities();
            return db;
        }
        /// <summary>
        /// 16位：ComputeHash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetMD5_Md(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] myData = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < myData.Length; i++)
            {
                sBuilder.Append(myData[i].ToString("x"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 32位加密：直接使用HashPasswordForStoringInConfigFile
        /// </summary>
        /// <param name="input"></param>
        /// <returns><</returns>
        public string GetMD5HashPassword_Md(string input)
        {
            string md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(input, "MD5");
            return md5.ToString();
        }
        public void ValueIsEmpty<T>(in T value)
        {
            string s = value.ToString();
            if (string.IsNullOrEmpty(s))
                ValueIsEmpty_Val = true; 
        }
        public bool ValueIsEmpty_Val { get; private set; }
        //Request _Header.cshtml部分檢視頁面(Views)預設定抓取當前頁面的網址並存進Session
        //沒有 _Header.cshtml部分檢視頁面則自動跳轉至首頁
        public string RedirectUrl {
            get
            {
                string url = "";
                //Session["driectUrl"] 設定值在_Header.cshtml
                if (Session["driectUrl"] != null)
                {
                    url = Convert.ToString(Session["driectUrl"].ToString() ?? "").Trim();
                }
                if (string.IsNullOrEmpty(url)) url = "/Home/Index";   //為空則回到首頁
                //如果回傳網址與當前位置相同則回到首頁
                if (url == currentUrl) url = "/Home/Index";
                Session["driectUrl"] = "";
                return url;
            }
            set
            {
                Session["driectUrl"] = value;
            }
        }
        //ResultMessage當前Controller與ActionName網址
        public string currentUrl { private get; set; }

        //清除登入資訊
        public void ClearUserInfo()
        {
            Session.RemoveAll();
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[InternalVal._COOKIEUSERINFO];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        //獲取Session的Account
        public string getSessionAccount_Val
        {
            get
            {
                string val = "";
                if (Session[InternalVal._SESSIONACCOUNT] != null)
                    val = Convert.ToString(Session[Other.InternalVal._SESSIONACCOUNT] ?? "").Trim();
                return val;
            }
        }


    }
}