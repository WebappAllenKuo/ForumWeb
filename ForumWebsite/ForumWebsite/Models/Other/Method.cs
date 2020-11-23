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
        //是否無值
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
                //預設為回到首頁
                string url = "/Home/Index";
                //Session["driectUrl"] 設定值在_Header.cshtml
                if (Session["driectUrl"] != null)
                {
                    url = (!Session["driectUrl"].Equals("")) ? Session["driectUrl"].ToString() : url;
                }
                //如果回傳網址與ResultMessage()位置相同則回到首頁
                if (url.Equals(currentUrl)) url = "/Home/Index";
                //如果未登錄則跳轉至首頁(UserLoginCheckAttribute)
                if (Session["notLoginDriectUrl"] != null)
                {
                    url = (!Session["notLoginDriectUrl"].Equals("")) ? Session["notLoginDriectUrl"].ToString() : url;
                    Session["notLoginDriectUrl"] = "";
                }
                Session["driectUrl"] = "";
                return url.Trim();
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
            HttpCookie cookie = System.Web.HttpContext.Current.Response.Cookies[InternalVal._COOKIEUSERINFO];
            if (cookie != null)
            {   

                //清除客戶端cookie登入資料
                cookie.Expires = DateTime.Now.AddDays(-1);
                //COOKIE寫入至客戶端
                //System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
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
        public string StrSubstring(string str, int start, int end)
        {
            if (str.Length > end)
                return (str != null && end > start) ? str.Substring(start, end - 1) + "..." : "發生錯誤!";
            else
                return str;
        }


    }
}