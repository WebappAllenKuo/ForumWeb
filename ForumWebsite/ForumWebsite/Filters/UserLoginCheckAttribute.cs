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
            Method_Cs Method = new Method_Cs();
            var objController = filterContext.Controller;
            var S_account = filterContext.HttpContext.Session[InternalVal._SESSIONACCOUNT];
            var C_userInfo = filterContext.HttpContext.Request.Cookies[InternalVal._COOKIEUSERINFO];
            string LoginName = Convert.ToString(S_account ?? "").Trim();
            //filterContext.HttpContext.Session["s01"] = 55;
            //filterContext.HttpContext.Session["s02"] = 56;
            if (LoginName == "" && C_userInfo != null)
            {
                //filterContext.HttpContext.Session["s01"] = C_userInfo[InternalVal._COOKIEACCOUNT];
                //filterContext.HttpContext.Session["s02"] = C_userInfo[InternalVal._COOKIEANAME];
                filterContext.HttpContext.Session[InternalVal._SESSIONACCOUNT] = C_userInfo[InternalVal._COOKIEACCOUNT];
                filterContext.HttpContext.Session[InternalVal._SESSIONNAME] = C_userInfo[InternalVal._COOKIEANAME];
                //清除客戶端cookie登入資料
                //C_userInfo.Value = "";
                //COOKIE更值須寫入才會變更
                filterContext.HttpContext.Response.Cookies.Add(C_userInfo);
            }
            S_account = filterContext.HttpContext.Session[InternalVal._SESSIONACCOUNT];
            S_account = Convert.ToString(S_account ?? "").Trim();
            if (string.IsNullOrEmpty(S_account.ToString()))
            {
                objController.TempData[InternalVal._RESULTMSG] = "請先登入會員!";
                filterContext.HttpContext.Session["notLoginDriectUrl"] = "/Home/Index";
                filterContext.HttpContext.Response.RedirectToRoute(new { controller = "Home", action = "ResultMessage" });
            }
        }
    }
}