using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using Interface.Forum.Repositories;
using ForumWebsite.Models;

namespace Repositories.Forum
{
    public class ArticleRepoBiz : IArticleRepo
    {
        ForumWebsiteDBEntities db = new ForumWebsiteDBEntities();
        private string _connection = new ConnectionFactory()._connectionString;
        #region 文章內容
        /// <summary>
        /// 文章總文章筆數
        /// </summary>
        /// <returns></returns>
        public int ArticleCount_Md()
        {
            using (var connection = new SqlConnection(_connection))
            {
                string strSql = "select Count(*) from article_tb";
                return (int)connection.ExecuteScalar(strSql);
            }
        }
        /// <summary>
        /// 獲取單筆文章內容
        /// </summary>
        /// <param name="id">article_id</param>
        /// <returns></returns>
        public article_Tb GetArticle_Md(int id)
        {
            article_Tb objItem = db.article_Tb.FirstOrDefault(s => s.arti_id == id);
            return objItem;
        }
        /// <summary>
        /// 獲取單筆文章內容
        /// </summary>
        /// <param name="numStr">文章流水編號</param>
        /// <returns></returns>
        public article_Tb GetArticle_Md(string numStr)
        {
            article_Tb objItem = db.article_Tb.FirstOrDefault(s => s.numNo == numStr);
            return objItem;
        }
        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        public List<article_Tb> ListArticle_Md(int? board_id, int? theme_id)
        {
            List<article_Tb> objItems = new List<article_Tb>();
            //預設查詢全部文章
            string strSql = "select * from article_tb";
            if (board_id != null && theme_id == null)
            {
                //某版區的全部文章
                strSql += " where board_id = " + int.Parse(board_id.ToString());
            }
            else if (theme_id != null)
            {
                //某版區的某單元全部文章
                strSql += " where board_id = " + int.Parse(board_id.ToString()) + " and arti_theme = " + int.Parse(theme_id.ToString());
            }
            using (var connection = new SqlConnection(_connection))
            {
                var result = connection.Query<article_Tb>(strSql).ToList();
                //判斷是否無資料
                if (result.Count() > 0)
                {
                    objItems = result;
                }
            }
            return objItems;
        }
        #region 文章內容-資料變更
        public bool InsertArticle_Md(article_Tb articleItem)
        {
            //為空則回傳false
            if (articleItem == null) return false;
            using (var connection = new SqlConnection(_connection))
            {
                string strSql = @"insert article_tb (
                                title, arti_date, arti_txt, view_num, arti_theme,
                                board_id, user_id, numNo) 
                            values(
                                @title, @arti_date, @arti_txt, @view_num, @arti_theme, 
                                @board_id, @user_id, @numNo)";
                articleItem.arti_date = DateTime.Now;
                articleItem.view_num = 0;
                //是否有資料變更
                return connection.Execute(strSql, articleItem) > 0;

            }
        }
        public bool UpdateArticle_Md(article_Tb articleItem)
        {
            article_Tb objItem = db.article_Tb.FirstOrDefault(m => m.arti_id == articleItem.arti_id);
            objItem.title = articleItem.title.ToString();
            objItem.arti_update = DateTime.Now;
            objItem.arti_txt = articleItem.arti_txt.ToString();
            objItem.arti_theme = int.Parse(articleItem.arti_theme.ToString());  //更換文章區塊
            return db.SaveChanges() > 0;
        }
        public bool DeleteArticle_Md(int articleId)
        {
            using (var connection = new SqlConnection(_connection))
            {
                //刪除文章
                string strSql = "delete from article_tb where arti_id = @id";
                //刪除回覆文章
                return connection.Execute(strSql, new { id = articleId }) > 0;
            }
        }
        #endregion
        #endregion
        #region 文章回覆
        //多筆文章回覆
        public List<article_reply_Tb> ListArticleReply_Md(int id)
        {
            List<article_reply_Tb> objItems = new List<article_reply_Tb>();
            using (var connection = new SqlConnection(_connection))
            {
                string strSql = "select *, ROW_NUMBER() OVER(order by reply_id) replyOrder from article_reply_Tb where arti_id = @id";
                objItems = connection.Query<article_reply_Tb>(strSql, new { id = id }).ToList();
                return objItems;
            }
        }
        //單筆文章回覆
        public article_reply_Tb GetArticleReply_Md(int? reply_id)
        {
            //判定是否有傳遞文章編號
            if (reply_id == null)
            {
                return null;
            }
            article_reply_Tb item = new article_reply_Tb();
            using (var connection = new SqlConnection(_connection))
            {
                string strSql = "select * from article_reply_Tb where reply_id = @id";
                item = connection.QueryFirstOrDefault<article_reply_Tb>(strSql, new { id = reply_id });
            }
            return item;
        }

        public int GetArticleReplyCount_Md(int artiId)
        {
            return db.article_reply_Tb.Count(m => m.arti_id == artiId);
        }

        #region 文章回覆-資料變更
        public bool InsertArticleReply_Md(article_reply_Tb addArticleReplyInfo)
        {
            article_reply_Tb objItem = new article_reply_Tb();
            objItem.reply_txt = addArticleReplyInfo.reply_txt;
            objItem.reply_date = addArticleReplyInfo.reply_date;
            objItem.reply_update = addArticleReplyInfo.reply_update;
            objItem.arti_id = addArticleReplyInfo.arti_id;
            objItem.user_id = addArticleReplyInfo.user_id;
            db.article_reply_Tb.Add(objItem);
            return db.SaveChanges() > 0;
        }

        public bool UpdateArticleReply_Md(article_reply_Tb ArticleReplyInfo)
        {
            using (var connection = new SqlConnection(_connection))
            {
                string strSql = @"update article_reply_Tb set reply_txt = @reply_txt, reply_update = @reply_update 
                where reply_id = @reply_id";
                ArticleReplyInfo.reply_update = DateTime.Now;
                return connection.Execute(strSql, ArticleReplyInfo) > 0;
            }
        }

        public bool DeleteArticleReply_Md(int articleReplyId)
        {
            article_reply_Tb objItem = GetArticleReply_Md(5);
            objItem = db.article_reply_Tb.Where(m => m.reply_id == objItem.reply_id).FirstOrDefault();
            db.article_reply_Tb.Remove(objItem);
            return db.SaveChanges() > 0;
        }
        #endregion
        #endregion
    }
}