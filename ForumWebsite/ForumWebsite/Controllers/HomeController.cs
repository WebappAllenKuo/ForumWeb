using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForumWebsite.Models.Service;
using ForumWebsite.Models;
using ForumWebsite.Models.Interface.Service;
using ForumWebsite.Models.Other;

namespace ForumWebsite.Controllers
{
    public class HomeController : Controller
    {
        protected IService_Board Service_Board_P { get; private set; }
        public HomeController() : this(null) { }
        public HomeController(IService_Board Service_Board_Val)
        {
            Service_Board_P = Service_Board_Val ?? new Service_Board();
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Header()
        {
            IList<board_Tb> objItem = Service_Board_P.ListBoard_Md();
            HomeViewModel HomeViewModel_P = new HomeViewModel();
            HomeViewModel_P.boardList = objItem;
            string account = Convert.ToString(Session[InternalVal._SESSIONACCOUNT] ?? "").Trim();
            ViewData["onLogin"] = false;    //預設未登入
            //是否已登入
            if (!string.IsNullOrEmpty(account))
            {
                ViewData["onLogin"] = true;
                ViewData["username"] = Convert.ToString(Session[InternalVal._SESSIONNAME].ToString());
            }
            return PartialView(HomeViewModel_P);
        }
        public ActionResult ResultMessage()
        {
            ViewData[InternalVal._RESULTMSG] = (TempData[InternalVal._RESULTMSG] == null) ? "無任何訊息!" : TempData[InternalVal._RESULTMSG]; 
            Method_Cs Method = new Method_Cs();
            //傳遞當前網址/User/ResultMessage
            Method.currentUrl = Request.Url.LocalPath;
            //傳遞至前端跳轉頁面
            string url = Method.RedirectUrl;
            ViewBag.Url = url;
            //if (string.IsNullOrEmpty(ViewBag.Url)) ViewBag.Url = "/";   //為空則回到首頁
            return View();
        }
    }
}