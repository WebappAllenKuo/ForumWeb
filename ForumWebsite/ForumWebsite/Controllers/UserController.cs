using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForumWebsite.Models;
using ForumWebsite.Models.Other;
using ForumWebsite.Models.Interface.Service;
using ForumWebsite.Models.Repository;
using ForumWebsite.Models.Service;
using ForumWebsite.Filters;

namespace ForumWebsite.Controllers
{
    public class UserController : Controller
    {
        private Method_Cs Method = new Method_Cs();
        protected IService_User Server_User_P { get; private set; }
        public UserController() : this(null) { }
        public UserController(IService_User Service_Users_Val)
        {
            Server_User_P = Service_Users_Val ?? new Service_User();
        }
        // GET: User
        #region 註冊會員
        public ActionResult UserRegistration()
        {
            if (Method.getSessionAccount_Val != "") {
                TempData[InternalVal._RESULTMSG] = "您已是會員!";
                //Method.RedirectUrl = "";
                return RedirectToAction("ResultMessage", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserRegistration(user_Tb form)
        {
            string passwdCompare = Request.Form["passwordCompare"].ToString().Trim();
            bool columnIsError = false;
            if (passwdCompare == "")
            {
                ViewBag.passwdCompareError = "確認密碼不可為空";
                columnIsError = true;
            }
            //帳號是否重複註冊
            var accountExist = Server_User_P.GetUserInfo_Md(form.account.Trim());
            if (accountExist != null)
            {
                ViewBag.accountError = "該帳號已被註冊";
                columnIsError = true;
            }
            //密碼比較
            string passwd = form.password.Trim() ?? null;
            if (passwdCompare != passwd && passwd != null)
            {
                ViewBag.passwdCompareError = "確認密碼不相同";
                columnIsError = true;
            }
            //如有填寫其他欄位位填寫則返回該值
            TempData["passwdCompare"] = passwdCompare;      //因"確認密碼"不存在於模型，而外設置
            if (!ModelState.IsValid)
            {
                return View("UserRegistration", form);
            }
            //研判是否有錯誤
            if (columnIsError)
            {
                return View("UserRegistration", form);
            }
            user_Tb objItem = new user_Tb()
            {
                account = form.account.Trim(),
                password = form.password.Trim(),
                username = form.username.Trim(),
                gender = form.gender,
                birthday = form.birthday,
                email = form.email.Trim(),
                account_right = "101"      //權限設置101:會員, 1011為管理者
            };
            TempData["objUserInfo"] = objItem;
            return RedirectToAction("ResultAddUser");
        }
        public ActionResult ResultAddUser()
        {
            user_Tb objItem = (user_Tb)TempData["objUserInfo"];
            //防止重整頁面重複提交
            if (objItem == null) return Redirect("/");
            var accountExist = Server_User_P.GetUserInfo_Md(objItem.account);
            if (accountExist == null)
            {
                bool Result = Server_User_P.InsertUser_Md(objItem);
                ViewData["Message"] = (Result) ? "加入會員成功!" : "加入會員失敗，請重新加入!!";
                ViewData["NoSuccess"] = false;
                ViewData["Message"] += "<br />" + ((Result) ? objItem.username + " 恭喜您成功加入會員!!" : "");
            }
            else
            {
                ViewData["Message"] = "非常抱歉您的帳號已被註冊，請重新註冊帳號!!";
            }
            return View(objItem);
        }
        #endregion
        #region 更新會員
        [UserLoginCheck]
        public ActionResult UpdateUserInfo(string user)
        {
            //檢查登入會員與SESSION是否相同
            if (user != Method.getSessionAccount_Val)
            {
                TempData[InternalVal._RESULTMSG] = "請先登入會員!";
                return RedirectToAction("ResultMessage", "Home");
            }
            user_Tb objItem = Server_User_P.GetUserInfo_Md(user);
            return View(objItem);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserLoginCheck]
        public ActionResult UpdateUserInfoCheck(user_Tb objItem)
        {
            //檢查是否已登入會員
            if (string.IsNullOrEmpty(Method.getSessionAccount_Val))
            {
                TempData[InternalVal._RESULTMSG] = "請先登入會員!";
                return RedirectToAction("ResultMessage", "Home");
            }
            //給予帳號與ID
            objItem.account = Method.getSessionAccount_Val;
            objItem.user_id = int.Parse(Request.Form["userId"].ToString());
            user_Tb userItem = new user_Tb();
            userItem = Server_User_P.GetUserInfo_Md(objItem.user_id);
            if (userItem.account != objItem.account)
            {
                TempData[InternalVal._RESULTMSG] = "會員資料更新失敗!";
                return RedirectToAction("ResultMessage", "Home");
            }
            //放入更新項目
            userItem.username = objItem.username;
            userItem.gender = objItem.gender;
            userItem.birthday = objItem.birthday;
            userItem.email = objItem.email;
            bool result = Server_User_P.UpdateUser_Md(userItem);
            string msg = "會員資料更新成功!<br />請重新登入會員!";
            if (!result) { msg = "會員資料更新失敗!"; }
            TempData[InternalVal._RESULTMSG] = msg;
            //設定返回位置
            //Method.RedirectUrl = "/User/UserInfoCenter";
            //清除登入資訊
            Method.ClearUserInfo();
            return RedirectToAction("ResultMessage", "Home");
        }
        public ActionResult Test00()
        {
            Response.Write("old:" + Session.SessionID);
            this.Session.Abandon();
            this.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty) { HttpOnly = true });
            Response.Write("new:" + Session.SessionID);
            return View();
        }
        #endregion
        #region 登入會員
        //[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin()
        {
            //檢查是否有登入過會員
            if (string.IsNullOrEmpty(Method.getSessionAccount_Val))
            {
                bool isError = false;
                string userName = Request.Form["userName"].ToString().Trim();
                string password = Request.Form["password"].ToString().Trim();
                //判斷欄位是否空值
                Method.ValueIsEmpty(userName);
                Method.ValueIsEmpty(password);
                //獲取結果
                isError = Method.ValueIsEmpty_Val;
                if (isError) return Redirect(Method.RedirectUrl);
                user_Tb objItem = Server_User_P.GetUserInfo_Md(userName);
                TempData[InternalVal._RESULTMSG] = "登入會員失敗!";
                //確認是否有該用戶
                if (objItem == null)
                {
                    return RedirectToAction("ResultMessage", "Home");
                } else if (!objItem.password.Equals(Method.GetMD5_Md(password)))
                {
                    //檢查密碼
                    TempData[InternalVal._RESULTMSG] = "帳號或密碼錯誤!";
                }
                if (objItem.account == userName && objItem.password.Equals(Method.GetMD5_Md(password)))
                {
                    HttpCookie cookie = new HttpCookie(InternalVal._COOKIEUSERINFO);
                    cookie.Values.Add(InternalVal._COOKIEACCOUNT, userName);
                    cookie.Values.Add(InternalVal._COOKIEANAME, objItem.username);
                    cookie.Values.Add("userVerify", Method.GetMD5HashPassword_Md(objItem.password));
                    //cookie保存2天
                    cookie.Expires = DateTime.Now.AddDays(2);
                    Response.Cookies.Add(cookie);
                    HttpCookie UserSession = new HttpCookie("UserSession");
                    UserSession.Values.Add("TestSessionID", Session.SessionID);
                    Session[InternalVal._SESSIONACCOUNT] = objItem.account;
                    Session[InternalVal._SESSIONNAME] = objItem.username;
                    cookie.Expires = DateTime.Now.AddDays(2);
                    Response.Cookies.Add(UserSession);
                    //給予SessionID
                    //Session["sessionID"] = Session.SessionID + userName;
                    //Session["sessionIdCompare"] = Session.SessionID + userName;
                    TempData[InternalVal._RESULTMSG] = Session[InternalVal._SESSIONACCOUNT] + " 已成功登入會員!";
                }
            }
            //Method.RedirectUrl = "~/Home/Index";
            //return View();
            return RedirectToAction("ResultMessage", "Home");
        }
        #endregion
        #region 登出會員
        public ActionResult OutLogin()
        {
            //清除登入資訊
            Method.ClearUserInfo();
            TempData[InternalVal._RESULTMSG] = "登出成功!";
            return RedirectToAction("ResultMessage", "Home");
        }
        #endregion
        #region 會員中心
        [UserLoginCheck]
        public ActionResult UserInfoCenter()
        {
            //if (string.IsNullOrEmpty(Method.getSessionAccount_Val))
            //{
            //    TempData[InternalVal._RESULTMSG] = "請先登入會員!";
            //    return RedirectToAction("ResultMessage", "Home");
            //}
            //if (!Method.getSessionAccount_Val.Equals(Session[InternalVal._SESSIONACCOUNT].ToString()))
            //{
            //    TempData[InternalVal._RESULTMSG] = "操作錯誤!請勿隨意更改參數!";
            //    return RedirectToAction("ResultMessage", "Home");
            //}
            return View();
        }
        #endregion
    }
}