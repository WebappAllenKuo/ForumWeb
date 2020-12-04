
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models;
using Interface.Forum.Repositories;
using Interface.Forum.Services;
using Repositories.Forum;

namespace Services.Forum
{
    public class ArticleServiceBiz : IArticleService
    {
        protected IArticleRepo Repository_Article_P { get; private set; }
        public ArticleServiceBiz() : this(null) { }
        public ArticleServiceBiz(IArticleRepo Repository_Article_Val)
        {
            Repository_Article_P = Repository_Article_Val ?? new ArticleRepoBiz();
        }

        #region 文章內容
        public List<article_Tb> ListArticle_Md(int? board_id, int? theme_id)
        {
            return Repository_Article_P.ListArticle_Md(board_id, theme_id);
        }
        public article_Tb GetArticle_Md(int id)
        {
            return Repository_Article_P.GetArticle_Md(id);
        }
        /// <summary>
        /// 獲取單筆文章內容
        /// </summary>
        /// <param name="numStr">文章流水編號</param>
        /// <returns></returns>
        public article_Tb GetArticle_Md(string numStr)
        {
            article_Tb objItem = Repository_Article_P.GetArticle_Md(numStr);
            return objItem;
        }
        public int ArticleCount_Md()
        {
            return Repository_Article_P.ArticleCount_Md();
        }
        #region 文章內容-資料異動
        public bool InsertArticle_Md(article_Tb articleItem)
        {
            return Repository_Article_P.InsertArticle_Md(articleItem);
        }
        public bool UpdateArticle_Md(article_Tb articleItem)
        {
            return Repository_Article_P.UpdateArticle_Md(articleItem);
        }
        public bool DeleteArticle_Md(int articleId)
        {
            return Repository_Article_P.DeleteArticle_Md(articleId);
        }
        #endregion
        #endregion

        #region 文章回覆
        public List<article_reply_Tb> ListArticleReply_Md(int id)
        {
            return Repository_Article_P.ListArticleReply_Md(id);
        }
        public article_reply_Tb GetArticleReply_Md(int? reply_id)
        {
            return Repository_Article_P.GetArticleReply_Md(reply_id);
        }
        public int GetArticleReplyCount_Md(int artiId)
        {
            return Repository_Article_P.GetArticleReplyCount_Md(artiId);
        }
        #region 文章回覆-資料異動
        public bool InsertArticleReply_Md(article_reply_Tb addArticleReplyInfo)
        {
            return Repository_Article_P.InsertArticleReply_Md(addArticleReplyInfo);
        }
        public bool UpdateArticleReply_Md(article_reply_Tb ArticleReplyInfo)
        {
            return Repository_Article_P.UpdateArticleReply_Md(ArticleReplyInfo);
        }
        public bool DeleteArticleReply_Md(int articleReplyId)
        {
            return Repository_Article_P.DeleteArticleReply_Md(articleReplyId);
        }
        #endregion
        #endregion
    }
}