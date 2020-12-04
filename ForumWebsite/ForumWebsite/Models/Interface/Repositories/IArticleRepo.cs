using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumWebsite.Models.Interface.Repositories
{
    public interface IArticleRepo
    {
        #region 文章內容
        /// <summary>
        /// 全部文章
        /// </summary>
        /// <returns></returns>
        List<article_Tb> ListArticle_Md(int? board_id, int? theme_id);
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
        /// <summary>
        /// 獲取單筆文章內容
        /// </summary>
        /// <param name="numStr">文章流水編號</param>
        /// <returns></returns>
        article_Tb GetArticle_Md(string numStr);

        #region 文章內容-資料異動
        /// <summary>
        /// 插入文章
        /// </summary>
        /// <param name="articleItem">文章模型資料</param>
        /// <returns></returns>
        bool InsertArticle_Md(article_Tb articleItem);
        /// <summary>
        /// 更新文章內容
        /// </summary>
        /// <param name="articleId">文章模型資料</param>
        /// <returns></returns>
        bool UpdateArticle_Md(article_Tb articleItem);
        /// <summary>
        /// 刪除文章內容
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        bool DeleteArticle_Md(int articleId);
        #endregion
        #endregion

        #region 文章回覆
        /// <summary>
        /// 單筆文章全部回覆
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<article_reply_Tb> ListArticleReply_Md(int id);
        /// <summary>
        /// 獲取單筆文章回覆
        /// </summary>
        /// <param name="id">文章編號</param>
        /// <returns></returns>
        article_reply_Tb GetArticleReply_Md(int? reply_id);
        /// <summary>
        /// 文章回覆數量
        /// </summary>
        /// <param name="artiId">文章編號</param>
        /// <returns></returns>
        int GetArticleReplyCount_Md(int artiId);

        #region 文章回覆-資料異動
        /// <summary>
        /// 插入文章回覆
        /// </summary>
        /// <param name="addArticleReplyInfo">插入回覆文章資訊</param>
        /// <returns></returns>
        bool InsertArticleReply_Md(article_reply_Tb addArticleReplyInfo);
        /// <summary>
        /// 更新文章回覆
        /// </summary>
        /// <param name="ArticleReplyInfo">文章回覆資訊</param>
        /// <returns></returns>
        bool UpdateArticleReply_Md(article_reply_Tb ArticleReplyInfo);
        /// <summary>
        /// 刪除文章回覆
        /// </summary>
        /// <param name="articleReplyId">文章回覆編號</param>
        /// <returns></returns>
        bool DeleteArticleReply_Md(int articleReplyId);
        #endregion
        #endregion
    }
}