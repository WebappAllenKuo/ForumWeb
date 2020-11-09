using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForumWebsite.Models.Other;

namespace ForumWebsite.Filters
{
    //檢查是否已登入會員，如果有給SESSION值
    public class UserLoginCheckAttribute : ActionFilterAttribute
    {
        public UserLoginCheckAttribute() { this.Order = int.MaxValue; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var objController = filterContext.Controller;
            var S_account = filterContext.HttpContext.Session[InternalVal._SESSIONACCOUNT];
            var C_userInfo = filterContext.HttpContext.Request.Cookies[InternalVal._COOKIEUSERINFO];
            string LoginName = Convert.ToString(S_account ?? "").Trim();
            if (LoginName == "" && C_userInfo != null)
            {
                string userName = C_userInfo[InternalVal._COOKIEACCOUNT].ToString();
                //如果cookie有資料並把資料傳遞給session
                S_account = userName;
                filterContext.HttpContext.Session[InternalVal._SESSIONNAME] = filterContext.HttpContext.Request.Cookies[InternalVal._COOKIEUSERINFO][InternalVal._COOKIEANAME].ToString();
                //清除cookie登入資料
                C_userInfo.Expires = DateTime.Now.AddDays(-1);
                //COOKIE寫入至客戶端
                filterContext.HttpContext.Request.Cookies.Add(C_userInfo);
                //給予SessionID
                //filterContext.HttpContext.Session["sessionID"] = filterContext.HttpContext.Session.SessionID + userName;
                //filterContext.HttpContext.Session["sessionIdCompare"] = filterContext.HttpContext.Session.SessionID + userName;
            }
            S_account = Convert.ToString(S_account ?? "").Trim();
        }
    }
}