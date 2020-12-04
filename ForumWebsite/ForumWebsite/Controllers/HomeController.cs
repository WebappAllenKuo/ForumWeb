using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForumWebsite.Models;
using ForumWebsite.Models.Interface.Services;
using ForumWebsite.Models.Other;
using System.Collections.Specialized;

namespace ForumWebsite.Controllers
{
    public class HomeController : Controller
    {   
        private Method_Cs Method = new Method_Cs();
        protected IUserService Service_User_P { get; private set; }
        protected IBoardService Service_Board_P { get; private set; }
        protected IArticleService Service_Article_P { get; private set; }
        public HomeController() : this(null, null, null) { }
        public HomeController(IUserService Service_User_Val, IBoardService Service_Board_Val, IArticleService Service_Article_Val)
        {
            Service_User_P = Service_User_Val ?? new UserServiceBiz();
            Service_Board_P = Service_Board_Val ?? new BoardServiceBiz();
            Service_Article_P = Service_Article_Val ?? new ArticleServiceBiz();
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Header()
        {
            IList<board_Tb> objItem = Service_Board_P.ListBoard_Md().OrderBy(m => m.id).ToList();
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
            //訊息內容
            ViewData[InternalVal._RESULTMSG] = (TempData[InternalVal._RESULTMSG] == null) ? "無任何訊息!" : TempData[InternalVal._RESULTMSG]; 
            Method_Cs Method = new Method_Cs();
            //傳遞當前網址/User/ResultMessage
            Method.currentUrl = Request.Url.PathAndQuery;
            //傳遞至前端跳轉頁面
            string url = Method.RedirectUrl;
            ViewBag.Url = url;
            //if (string.IsNullOrEmpty(ViewBag.Url)) ViewBag.Url = "/";   //為空則回到首頁
            return View();
        }
        public PartialViewResult _RightArticleReply()
        {
            HomeViewModel viewModel = new HomeViewModel();
            //挑選出有回覆的文章
            List<article_Tb> articleItems = Service_Article_P.ListArticle_Md(null, null).Take(5).ToList();
            List<article_reply_Tb> articleReplyInfoList = new List<article_reply_Tb>();
            List<user_Tb> userInfoList = new List<user_Tb>();
            List<int> replyCountList = new List<int>();
            List<board_Tb> boardList = new List<board_Tb>();
            viewModel.articleList = articleItems;
            int num = 0;
            foreach (var item in articleItems )
            {
                num = Service_Article_P.GetArticleReplyCount_Md(item.arti_id);
                if (num > 0)
                {
                    article_reply_Tb articleReplyInfo = Service_Article_P.ListArticleReply_Md(item.arti_id).OrderByDescending(m => m.reply_id).FirstOrDefault();
                    articleReplyInfoList.Add(articleReplyInfo);
                    user_Tb userInfo = Service_User_P.GetUserInfo_Md(articleReplyInfo.user_id);
                    userInfoList.Add(userInfo);
                    boardList.Add(new board_Tb
                    {
                        id = (int)item.arti_theme,
                        theme_board_id = item.board_id
                    });
                }
                //獲取當前討論版
                string strParameter = "";
                // 取得當前網址集合
                NameValueCollection nvc = Request.QueryString;
                var board = nvc["board"];
                var theme = nvc["theme"];
                if (board != null && theme != null)
                    strParameter += "?board=" + board + "&theme=" + theme;
                else if(board != null && theme == null)
                    strParameter += "?board=" + board;
                ViewBag.urlPara = strParameter;
                replyCountList.Add(num);
            }
            viewModel.replyCountList = replyCountList;
            viewModel.replyAritlceList = articleReplyInfoList;
            viewModel.userInfoList = userInfoList;
            viewModel.boardList = boardList;
            return PartialView(viewModel);
        }
        public PartialViewResult _NewArticle()
        {
            HomeViewModel viewModel = new HomeViewModel();
            List<article_Tb> articleList = new List<article_Tb>();
            List<board_Tb> boardList = new List<board_Tb>();
            List<user_Tb> userList = new List<user_Tb>();
            //標題字數
            articleList = Service_Article_P.ListArticle_Md(null, null).OrderByDescending(m=> m.arti_date).Take(5).ToList();
            viewModel.articleList = articleList;
            for (int i = 0; i < articleList.Count(); i++)
            {
                articleList[i].title = Method.StrSubstring(articleList[i].title, 0, 50);
                articleList[i].arti_txt = Method.StrSubstring(articleList[i].arti_txt, 0, 100);
                boardList.Add(new board_Tb()
                {
                    id = (int)articleList[i].arti_theme,
                    theme_board_id = articleList[i].board_id,
                    board_name = Service_Board_P.GetBoardName_Md(articleList[i].board_id)
                });
                userList.Add(new user_Tb()
                {
                    user_id = articleList[i].user_id,
                    username = Service_User_P.GetUserInfo_Md(articleList[i].user_id).username
                });
            }
            viewModel.articleList = articleList;
            viewModel.boardList = boardList;
            viewModel.userInfoList = userList;
            return PartialView(viewModel);
        }
        public PartialViewResult _LeftHotArticle()
        {
            HomeViewModel viewModel = new HomeViewModel();
            List<article_Tb> articleList = new List<article_Tb>();
            List<board_Tb> boardList = new List<board_Tb>();
            List<user_Tb> userList = new List<user_Tb>();
            List<article_reply_Tb> articleReplyList = new List<article_reply_Tb>();
            articleList = Service_Article_P.ListArticle_Md(null, null).OrderByDescending(m => m.view_num).ThenByDescending(m => m.arti_id).Take(5).ToList();
            
            //文章最新回覆的用戶ID
            article_reply_Tb articleReplyItem = new article_reply_Tb();
            user_Tb userItem = new user_Tb();
            for (int i = 0 ; i < articleList.Count(); i++)
            {
                //標題字數
                articleList[i].title = Method.StrSubstring(articleList[i].title, 0, 30);
                //討論版名稱
                boardList.Add(new board_Tb() {
                    id = (int)articleList[i].arti_theme,
                    board_name = Service_Board_P.GetBoardName_Md(articleList[i].board_id),
                    theme_board_id = articleList[i].board_id
                }) ;
                articleReplyItem = Service_Article_P.ListArticleReply_Md(articleList[i].arti_id).OrderByDescending(m => m.reply_id).FirstOrDefault();
                if (articleReplyItem != null)
                {
                    userItem = Service_User_P.GetUserInfo_Md(articleReplyItem.user_id);
                    userList.Add(userItem);
                    articleReplyList.Add(articleReplyItem);
                }
            }
            viewModel.articleList = articleList;
            viewModel.boardList = boardList;
            viewModel.userInfoList = userList;
            viewModel.replyAritlceList = articleReplyList;
            return PartialView(viewModel);
        }
    }
}