using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForumWebsite.Models;
using ForumWebsite.Models.Interface.Services;
using ForumWebsite.Models.Other;
using ForumWebsite.Filters;
using System.Collections.Specialized;
using Services.Forum;

namespace ForumWebsite.Controllers
{
    public class ArticleController : Controller
    {
        private Method_Cs Method = new Method_Cs();
        protected IUserService Service_User_P { get; private set; }
        protected IBoardService Service_Board_P { get; private set; }
        protected IArticleService Service_Article_P { get; private set; }
        public ArticleController() : this(null, null, null) { }
        public ArticleController(IUserService Service_User_Val, IBoardService Service_Board_Val, IArticleService Service_Article_Val)
        {
            Service_User_P = Service_User_Val ?? new UserServiceBiz();
            Service_Board_P = Service_Board_Val ?? new BoardServiceBiz();
            Service_Article_P = Service_Article_Val ?? new ArticleServiceBiz();
        }
        /// <summary>
        /// 討論版文章列表
        /// </summary>
        /// <param name="board"></param>
        /// <param name="theme"></param>
        /// <returns></returns>
        public ActionResult BoardArticle(int? board, int? theme)
        {
            HomeViewModel viewModel = new HomeViewModel();
            List<board_Tb> themeItems = new List<board_Tb>();
            viewModel.boardItem = new board_Tb();
            //如果board參數無值以及負數導向首頁
            Method.ValueIsEmpty(board);
            if (Method.ValueIsEmpty_Val || board < 0) return RedirectToAction("Index", "Home");
            ViewBag.boardId = board;
            //如果theme參數無值以及負數導向該版全部主題
            Method.ValueIsEmpty(theme);
            if (Method.ValueIsEmpty_Val || theme < 0) theme = 0;
            //目前討論版資訊
            viewModel.boardItem.id = (int)board;
            viewModel.boardItem.board_name = Service_Board_P.GetBoardName_Md((int)board);
            //獲取討論版分類項目
            themeItems = Service_Board_P.ListTheme_Md((int)board);
            viewModel.boardList = themeItems;
            //獲取全部主題或該項目文章列表
            List<article_Tb> articleItems = new List<article_Tb>();
            articleItems = (theme != 0) ? Service_Article_P.ListArticle_Md(board, theme).OrderByDescending(m => m.arti_date).ToList() : Service_Article_P.ListArticle_Md(board, null).OrderByDescending(m => m.arti_date).ToList();
            viewModel.articleList = articleItems;
            List<user_Tb> userItems = new List<user_Tb>();
            viewModel.replyCountList = new List<int>();
            //各文章回覆數量
            for (int i=0; i < articleItems.Count(); i++)
            {
                articleItems[i].title = Method.StrSubstring(articleItems[i].title, 0, 50);
                articleItems[i].arti_txt = Method.StrSubstring(articleItems[i].arti_txt, 0, 100);
                int sum = Service_Article_P.GetArticleReplyCount_Md(articleItems[i].arti_id);
                viewModel.replyCountList.Add(sum);
                userItems.Add(Service_User_P.GetUserInfo_Md(articleItems[i].user_id));
            }
            viewModel.userInfoList = userItems;
            return View(viewModel);
        }
        /// <summary>
        /// 觀看文章
        /// </summary>
        /// <param name="article"></param>
        /// <param name="board"></param>
        /// <param name="theme"></param>
        /// <returns></returns>
        public ActionResult ReviewArticle(int? article, int? board, int? theme)
        {
            HomeViewModel viewModel = new HomeViewModel();
            //文章ID為空直接跳轉至該討論版
            Method.ValueIsEmpty(article);
            if (Method.ValueIsEmpty_Val)
                return RedirectToAction("BoardArticle", new { board = board });
            //討論版ID為空直接跳轉至首頁
            Method.ValueIsEmpty(board);
            if (Method.ValueIsEmpty_Val)
                return RedirectToAction("Index", "Home");
            article_Tb articleItem = new article_Tb();
            articleItem = Service_Article_P.GetArticle_Md((int)article);
            //檢查該筆文章是否符合
            if (articleItem.arti_id != article || articleItem.board_id != board || articleItem.arti_theme != theme)
            {
                TempData[InternalVal._RESULTMSG] = "無該筆文章!";
                return RedirectToAction("ResultMessage", "Home");
            }
            //獲取當前的URL參數
            TempData["ArticleUrl"] = Request.Url.Query;
            TempData["ArticleQueryString"] = Request.QueryString;
            //文章內容資訊
            viewModel.articleItem = articleItem;
            viewModel.replyCount = Service_Article_P.GetArticleReplyCount_Md((int)article);
            viewModel.boardItem = new board_Tb() {
                id = (int)board,
                board_name = Service_Board_P.GetBoardName_Md((int)board)
            };
            viewModel.userItem = Service_User_P.GetUserInfo_Md(articleItem.user_id);
            //文章回覆
            List<article_reply_Tb> replyArticleItems = Service_Article_P.ListArticleReply_Md((int)article).ToList();
            viewModel.replyAritlceList = replyArticleItems;
            List<user_Tb> userItems = new List<user_Tb>();
            for (int i=0; i< replyArticleItems.Count(); i++)
            {
                userItems.Add(Service_User_P.GetUserInfo_Md(replyArticleItems[i].user_id));
            }
            viewModel.userInfoList = userItems;
            return View(viewModel);
        }
        /// <summary>
        /// 發佈文章
        /// </summary>
        /// <param name="board"></param>
        /// <param name="theme"></param>
        /// <returns></returns>
        [UserLoginCheck]
        public ActionResult LaunchArticle(int? board, int? theme)
        {
            HomeViewModel viewModel = new HomeViewModel();
            viewModel.boardList = new List<board_Tb>();
            board_Tb boardItem = new board_Tb();
            List<board_Tb> themeItems = new List<board_Tb>();
            //討論版ID如為空
            Method.ValueIsEmpty(board);
            if (Method.ValueIsEmpty_Val)
            {
                //獲取全部討論版，在選取後由AJAX取得分類(LaunchArticleTheme)
                viewModel.boardList = Service_Board_P.ListBoard_Md().OrderBy(m => m.id).ToList();
            }else
            {
                //當前討論版資訊
                Method.ValueIsEmpty(theme);
                if (!Method.ValueIsEmpty_Val)
                    boardItem.id = (int)theme;
                boardItem.theme_board_id = (int)board;
                boardItem.board_name = Service_Board_P.GetBoardName_Md((int)board);
                //獲取討論版分類
                themeItems = Service_Board_P.ListTheme_Md((int)board);
                //無討論版分類

                //themeItems = Service_Board_P.ListTheme_Md((int)board);
            }
            viewModel.themeList = themeItems;
            viewModel.boardItem = boardItem;
            viewModel.articleItem = new article_Tb();
            Session["ViewModel"] = viewModel;
            
            return View(viewModel);
        }
        /// <summary>
        /// AJAX動態生成討論版分類
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public JsonResult LaunchArticleTheme(int board)
        {
            List<board_Tb> boardItems = new List<board_Tb>();
            boardItems = Service_Board_P.ListTheme_Md(board);
            List<object> Date = new List<object>();
            foreach (var item in boardItems)
            {
                Date.Add(new { 
                    id = item.id,
                    theme_name = item.theme_name
                });
            }
            return Json(Date, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 檢查新發佈文章
        /// </summary>
        /// <param name="title"></param>
        /// <param name="board_id"></param>
        /// <param name="theme_id"></param>
        /// <param name="article_txt"></param>
        /// <returns></returns>
        [HttpPost]
        [UserLoginCheck]
        [ValidateAntiForgeryToken]
        public ActionResult CheckArticleInfo(string title, string board_id, string theme_id, string article_txt)
        {
            Method.ValueIsEmpty(title);
            Method.ValueIsEmpty(board_id);
            Method.ValueIsEmpty(theme_id);
            Method.ValueIsEmpty(article_txt);
            if (Method.ValueIsEmpty_Val)
            {
                return View("LaunchArticle", (HomeViewModel)Session["ViewModel"]);
            }
            //int getBoardId = Service_Board_P.GetBoardIdOfTheme_Md(int.Parse(theme_id.Trim()));
            int getBoardId = Service_Board_P.GetBoardIdOfTheme_Md(int.Parse(theme_id.Trim()));
            if (getBoardId != int.Parse(board_id.Trim()))
            {
                TempData[InternalVal._RESULTMSG] = "該討論版無此分類!請重新發佈文章!";
                return RedirectToAction("ResultMessage", "Home");
            }
            article_Tb articleItem = new article_Tb() {
                title = title.Trim(),
                arti_txt = article_txt.Trim(),
                board_id = int.Parse(board_id.Trim()),
                arti_theme = int.Parse(theme_id.Trim()),
                user_id = Service_User_P.GetUserInfo_Md(Session[InternalVal._SESSIONACCOUNT].ToString()).user_id
            };
            return RedirectToAction("AddArticle", articleItem);
        }
        /// <summary>
        /// 將新文章寫入資料庫
        /// </summary>
        /// <param name="articleItem"></param>
        /// <returns></returns>
        [UserLoginCheck]
        public ActionResult AddArticle(article_Tb articleItem)
        {
            if (articleItem == null)
            {
                TempData[InternalVal._RESULTMSG] = "發生錯誤!請重新發佈文章!";
                return RedirectToAction("ResultMessage", "Home");
            }
            //產生流水號
            string str = "";
            str += "AR000";
            str += DateTime.Now.ToString("yyMM");
            str += DateTime.Now.ToString("dd");
            str += new Random().Next(10, 99);
            str += DateTime.Now.ToString("hh");
            str += DateTime.Now.ToString("mm");
            str += DateTime.Now.ToString("ss");
            str += new Random().Next(10, 99);
            articleItem.numNo = str;
            bool result = Service_Article_P.InsertArticle_Md(articleItem);
            if(result)
                TempData[InternalVal._RESULTMSG] = "文章發佈成功!";
            else
                TempData[InternalVal._RESULTMSG] = "文章發佈失敗!";
            TempData["ArticleNO"] = str;
            article_Tb Item = Service_Article_P.GetArticle_Md(str);
            return RedirectToAction("ReviewArticle", new { article = Item.arti_id, board = articleItem.board_id, theme = articleItem.arti_theme });
        }
        public ActionResult EditArticle(int article, int board, int theme)
        {
            HomeViewModel viewModel = new HomeViewModel();
            board_Tb boardItem = new board_Tb();
            article_Tb articleItem = new article_Tb();
            List<board_Tb> themeItems = new List<board_Tb>();

            articleItem = Service_Article_P.GetArticle_Md(article);
            boardItem = Service_Board_P.BoardItem_Md(null, theme);
            boardItem.id = theme;
            boardItem.theme_board_id = board;
            boardItem.board_name = Service_Board_P.GetBoardName_Md(board);
            themeItems = Service_Board_P.ListTheme_Md(board);
            viewModel.articleItem = articleItem;
            viewModel.boardItem = boardItem;
            viewModel.themeList = themeItems;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditArticles(FormCollection form)
        {
            int boardId = 0;
            bool resultBoardId = int.TryParse(form["boardId"].ToString(), out boardId);
            article_Tb Item = new article_Tb();
            Item = Service_Article_P.GetArticle_Md(boardId);
            Item.title = form["title"].Trim();
            int themeId = 0;
            bool resultthemeId = int.TryParse(form["theme_id"].ToString().Trim(), out themeId);
            if(resultthemeId)
                Item.arti_theme = themeId;
            Item.arti_txt = form["article_txt"];
            bool result = Service_Article_P.UpdateArticle_Md(Item);
            //如有更改theme則更改返回頁面時Url的theme值
            NameValueCollection nvc = (NameValueCollection)TempData["ArticleQueryString"];
            string url="?";
            foreach(string key in nvc.Keys)
            {
                url += key + "=";
                url += (!key.Equals("theme")) ? nvc[key] : themeId.ToString();
                url += "&";
            }
            //收尾
            url += "order=0";
            Method.RedirectUrl = "/Article/ReviewArticle" + url;
            TempData[InternalVal._RESULTMSG] = "文章修改成功!";
            return RedirectToAction("ResultMessage", "Home");
        }
            /// <summary>
            /// 回覆文章
            /// </summary>
            /// <param name="article"></param>
            /// <param name="board"></param>
            /// <param name="theme"></param>
            /// <param name="reply"></param>
            /// <returns></returns>
            [UserLoginCheck]
        public ActionResult ReplyArticle(int? article, int? board, int? theme, int? reply)
        {
            //判斷是否有文章編號
            Method.ValueIsEmpty(article);
            if (Method.ValueIsEmpty_Val)
            {
                return RedirectToAction("Index", "Home");
            }
            HomeViewModel viewModel = new HomeViewModel();
            article_Tb articleItem = new article_Tb();
            article_reply_Tb replyItem = new article_reply_Tb();
            int num = (int)article;
            articleItem = Service_Article_P.GetArticle_Md(num);
            articleItem.title = Method.StrSubstring(articleItem.title, 0, 50);
            board_Tb boardItem = new board_Tb();
            boardItem = Service_Board_P.BoardItem_Md(null, theme);
            viewModel.replyCount = Service_Article_P.GetArticleReplyCount_Md((int)article);
            //回覆樓主
            TempData["replyAuthor"] = true;
            if (boardItem.theme_board_id != board && boardItem.id != theme)
            {
                TempData[InternalVal._RESULTMSG] = "發生錯誤!無法回覆文章!";
                return RedirectToAction("ResultMessage", "Home");
            }
            if (reply != null)
            {
                replyItem = Service_Article_P.GetArticleReply_Md(reply);
                articleItem.arti_txt = "\"引用:" + Method.StrSubstring(replyItem.reply_txt, 0, 30)+"\"";
                TempData["replyAuthor"] = false;
            }
            boardItem.theme_board_id = articleItem.board_id;
            boardItem.id = (int)theme;
            boardItem.board_name = Service_Board_P.GetBoardName_Md((int)board);
            boardItem.theme_name = boardItem.theme_name;
            viewModel.articleItem = articleItem;
            viewModel.boardItem = boardItem;
            return View(viewModel);
        }
        /// <summary>
        /// 檢查文章回覆內容與發佈
        /// </summary>
        /// <param name="article_txt"></param>
        /// <returns></returns>
        [HttpPost]
        [UserLoginCheck]
        public ActionResult CheckReplyInfo(string article_txt)
        {
            Method.ValueIsEmpty(article_txt);
            //檢查articleId是否空白
            Method.ValueIsEmpty(Request.Form["article"]);
            if (Method.ValueIsEmpty_Val)
            {
                TempData[InternalVal._RESULTMSG] = "回覆不可空白!" + Method.RedirectUrl;
                return RedirectToAction("ResultMessage", "Home");
            }
            user_Tb userItem = new user_Tb();
            userItem = Service_User_P.GetUserInfo_Md(Session[InternalVal._SESSIONACCOUNT].ToString());
            article_reply_Tb articleReplyItem = new article_reply_Tb();
            articleReplyItem.reply_txt = article_txt;
            articleReplyItem.reply_date = DateTime.Now;
            articleReplyItem.reply_update = DateTime.Now;
            articleReplyItem.arti_id = int.Parse(Request.Form["article"].Trim().ToString());
            articleReplyItem.user_id = userItem.user_id;
            bool result = Service_Article_P.InsertArticleReply_Md(articleReplyItem);
            //前往回覆的文章
            Method.RedirectUrl = "/Article/ReviewArticle" + TempData["ArticleUrl"];
            TempData[InternalVal._RESULTMSG] = (result) ? "回覆成功!" : "回覆失敗!";
            return RedirectToAction("ResultMessage", "Home");
        }
        /// <summary>
        /// 修改文章回覆
        /// </summary>
        /// <param name="replyId"></param>
        /// <param name="board"></param>
        /// <param name="theme"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [UserLoginCheck]
        public ActionResult EditReply(int replyId, int board, int theme, string account)
        {
            Method.ValueIsEmpty(replyId);
            Method.ValueIsEmpty(account);
            if (Method.ValueIsEmpty_Val)
            {
                TempData[InternalVal._RESULTMSG] = "回覆資訊有誤!";
                return RedirectToAction("ResultMessage", "Home");
            }
            article_reply_Tb replyArticle = new article_reply_Tb();
            replyArticle = Service_Article_P.GetArticleReply_Md(replyId);
            user_Tb user = new user_Tb();
            user = Service_User_P.GetUserInfo_Md(replyArticle.user_id);
            HomeViewModel viewModel = new HomeViewModel();
            viewModel.replyAritlceItem = replyArticle;
            viewModel.userItem = user;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditReply(int replyId, int? theme, string article_txt)
        {
            article_reply_Tb replyItem = new article_reply_Tb();
            replyItem = Service_Article_P.GetArticleReply_Md(replyId);
            replyItem.reply_txt = article_txt;
            Service_Article_P.UpdateArticleReply_Md(replyItem);
            string articleUrl = (string)TempData["ArticleUrl"];
            //前往回覆的文章
            Method.RedirectUrl = "/Article/ReviewArticle" + articleUrl;
            //TempData[InternalVal._RESULTMSG] = (result) ? "回覆成功!" : "回覆失敗!";
            TempData[InternalVal._RESULTMSG] = "修改回覆成功!";
            return RedirectToAction("ResultMessage", "Home");
        }
        /// <summary>
        /// 刪除文章回覆
        /// </summary>
        /// <param name="replyId"></param>
        /// <param name="article"></param>
        /// <returns></returns>
        public ActionResult DeleteReply(int replyId, int article)
        {
            HomeViewModel viewModel = new HomeViewModel();
            article_reply_Tb replyItem = new article_reply_Tb();
            article_Tb articleItem = new article_Tb();
            replyItem = Service_Article_P.GetArticleReply_Md(replyId);
            articleItem = Service_Article_P.GetArticle_Md(article);
            articleItem.title = Method.StrSubstring(articleItem.title, 0, 30);
            viewModel.replyAritlceItem = replyItem;
            viewModel.articleItem = articleItem;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult DeleteReply(int replyId)
        {
            bool result = Service_Article_P.DeleteArticleReply_Md(replyId);
            Method.RedirectUrl = "/Article/ReviewArticle" + TempData["ArticleUrl"];
            TempData[InternalVal._RESULTMSG] = "刪除成功!" + result;
            return RedirectToAction("ResultMessage", "Home");
        }
    }

}