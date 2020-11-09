using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumWebsite.Models.Interface.Repository
{
    public interface IRepository_Article
    {
        #region 文章內容
        /// <summary>
        /// 全部文章
        /// </summary>
        /// <returns></returns>
        List<article_Tb> ListArticle_Md(int? theme);
        /// <summary>
        /// 文章數量
        /// </summary>
        /// <returns></returns>
        int ArticleCount_Md();
        /// <summary>
        /// 單筆文章資訊
        /// </summary>
        /// <param name="id">文章編號</param>
        /// <returns></returns>
        article_Tb GetArticle_Md(int id);

        #region 文章內容-資料異動
        /// <summary>
        /// 插入文章
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool InsertArticle(int userId);
        /// <summary>
        /// 更新文章內容
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        bool UpdateArticle(int articleId);
        /// <summary>
        /// 刪除文章內容
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        bool DeleteArticle(int articleId);
        #endregion
        #endregion

        #region 文章回覆
        /// <summary>
        /// 單筆文章全部回覆
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<article_reply_Tb> ListArticleReply(int id);
        /// <summary>
        /// 獲取單筆文章回覆
        /// </summary>
        /// <param name="id">文章編號</param>
        /// <returns></returns>
        article_reply_Tb GetArticleReply(int? id);
        /// <summary>
        /// 文章回覆數量
        /// </summary>
        /// <param name="id">文章編號</param>
        /// <returns></returns>
        int GetArticleReplyCount(int id);

        #region 文章回覆-資料異動
        /// <summary>
        /// 插入文章回覆
        /// </summary>
        /// <param name="addArticleReplyInfo">插入回覆文章資訊</param>
        /// <returns></returns>
        bool InsertArticleReply(article_reply_Tb addArticleReplyInfo);
        /// <summary>
        /// 更新文章回覆
        /// </summary>
        /// <param name="articleReplyId">文章回覆編號</param>
        /// <returns></returns>
        bool UpdateArticleReply(int articleReplyId);
        /// <summary>
        /// 刪除文章回覆
        /// </summary>
        /// <param name="articleReplyId">文章回覆編號</param>
        /// <returns></returns>
        bool DeleteArticleReply(int articleReplyId);
        #endregion
        #endregion
    }
}