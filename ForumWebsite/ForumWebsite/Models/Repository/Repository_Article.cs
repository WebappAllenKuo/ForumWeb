using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models.Interface.Repository;
using ForumWebsite.Models;
using ForumWebsite.Models.SQLHelper;
using System.Data;
using System.Data.SqlClient;

namespace ForumWebsite.Models.Repository
{
    public class Repository_Article : IRepository_Article
    {
        ForumWebsiteDBEntities db = new ForumWebsiteDBEntities();
        
        #region 文章內容
        /// <summary>
        /// 文章總文章筆數
        /// </summary>
        /// <returns></returns>
        public int ArticleCount_Md()
        {
            string strSql = "select Count(*) from article_tb";
            return (int)SqlHelper.ExecuteScalar(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, null);
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
            string strSql = "";
            if (board_id == null && theme_id == null)
            {
                //全部文章
                strSql = "select * from article_tb";
            }
            else if (board_id != null && theme_id == null)
            {
                //某版區的全部文章
                strSql = "select * from article_tb where board_id = " + int.Parse(board_id.ToString());
            }
            else if(theme_id != null)
            {
                //某版區的某單元全部文章
                strSql = "select * from article_tb where board_id = " + int.Parse(board_id.ToString()) +" and arti_theme = " + int.Parse(theme_id.ToString());
            }
            using (SqlDataReader sqlDataReader = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, null))
            {
                //判斷是否無資料
                if (sqlDataReader.FieldCount > 0)
                {
                    while (sqlDataReader.Read())
                    {
                        objItems.Add(new article_Tb() {
                            arti_id = int.Parse(sqlDataReader["arti_id"].ToString()),
                            title = sqlDataReader["title"].ToString(),
                            arti_date = DateTime.Parse(sqlDataReader["arti_date"].ToString()),
                            arti_update = DateTime.Parse(sqlDataReader["arti_update"].ToString()),
                            arti_txt = sqlDataReader["arti_txt"].ToString(),
                            view_num = int.Parse(sqlDataReader["view_num"].ToString()),
                            arti_top = bool.Parse(sqlDataReader["arti_top"].ToString()),
                            arti_theme = int.Parse(sqlDataReader["arti_theme"].ToString()),
                            board_id = int.Parse(sqlDataReader["board_id"].ToString()),
                            user_id = int.Parse(sqlDataReader["user_id"].ToString()),
                            numNo = sqlDataReader["numNo"].ToString()
                        });
                    }
                }
            }
            return objItems;
        }
        #region 文章內容-資料變更
        public bool InsertArticle_Md(article_Tb articleItem)
        {
            //為空則回傳false
            if (articleItem == null) return false;
            string strSql = "insert article_tb (title, arti_date, arti_txt, view_num, arti_theme, board_id, user_id, numNo) " +
                "values(@title, @arti_date, @arti_txt, @view_num, @arti_theme, @board_id, @user_id, @numNo)";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@title", SqlDbType.NChar),
                new SqlParameter("@arti_date", SqlDbType.DateTime),
                new SqlParameter("@arti_txt", SqlDbType.NText),
                new SqlParameter("@view_num", SqlDbType.Int),
                new SqlParameter("@arti_theme", SqlDbType.Int),     //某版區塊文章
                new SqlParameter("@board_id", SqlDbType.Int),       //全部文章
                new SqlParameter("@user_id", SqlDbType.Int),
                new SqlParameter("@numNo", SqlDbType.NVarChar)
            };
            sqlParameters[0].Value = articleItem.title.ToString();
            sqlParameters[1].Value = DateTime.Now;
            sqlParameters[2].Value = articleItem.arti_txt.ToString();
            sqlParameters[3].Value = 0;
            sqlParameters[4].Value = int.Parse(articleItem.arti_theme.ToString());
            sqlParameters[5].Value = int.Parse(articleItem.board_id.ToString());
            sqlParameters[6].Value = int.Parse(articleItem.user_id.ToString());
            sqlParameters[7].Value = articleItem.numNo.ToString();
            //是否有資料變更
            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 0;
            
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

            //刪除文章
            string strSql = "delete from article_tb where arti_id = @id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", SqlDbType.Int)
            };
            sqlParameters[0].Value = articleId;
            //刪除回覆文章

            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 0;
        }
        #endregion
        #endregion
        #region 文章回覆
        //多筆文章回覆
        public List<article_reply_Tb> ListArticleReply_Md(int id)
        {
            List<article_reply_Tb> objItems = new List<article_reply_Tb>();
            string strSql = "select *, ROW_NUMBER() OVER(order by reply_id) replyOrder from article_reply_Tb where arti_id = @id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", SqlDbType.Int)
            };
            sqlParameters[0].Value = id;

            using (SqlDataReader sqlDataReader = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters))
            {
                while(sqlDataReader.Read())   //空值無資料
                {
                    objItems.Add(new article_reply_Tb()
                    {
                        reply_id = int.Parse(sqlDataReader["reply_id"].ToString()),
                        reply_txt = sqlDataReader["reply_txt"].ToString(),
                        reply_date = DateTime.Parse(sqlDataReader["reply_date"].ToString()),
                        reply_update = DateTime.Parse(sqlDataReader["reply_update"].ToString()),
                        arti_id = int.Parse(sqlDataReader["arti_id"].ToString()),
                        user_id = int.Parse(sqlDataReader["user_id"].ToString()),
                        replyOrder = sqlDataReader["replyOrder"].ToString()
                    });
                }
            }
            return objItems;
        }
        //單筆文章回覆
        public article_reply_Tb GetArticleReply_Md(int? reply_id)
        {
            //判定是否有傳遞文章編號
            if(reply_id == null)
            {
                return null;
            }
            article_reply_Tb item = new article_reply_Tb();
            string strSql = "select * from article_reply_Tb where reply_id = @id";
            SqlParameter[] objSqlParameter = new SqlParameter[]{
                new SqlParameter("@id", SqlDbType.Int)
            };
            objSqlParameter[0].Value = reply_id;
            using (SqlDataReader sqlDataReader = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, objSqlParameter))
            {
                if (sqlDataReader.Read()) {
                    item.reply_id = int.Parse(sqlDataReader["reply_id"].ToString());
                    item.reply_txt = sqlDataReader["reply_txt"].ToString();
                    item.reply_date = DateTime.Parse(sqlDataReader["reply_date"].ToString());
                    item.reply_update = DateTime.Parse(sqlDataReader["reply_update"].ToString());
                    item.arti_id = int.Parse(sqlDataReader["arti_id"].ToString());
                    item.user_id = int.Parse(sqlDataReader["user_id"].ToString());
                    //item.user_Tb.user_id = int.Parse(sqlDataReader["user_id"].ToString());
                    //item.user_Tb.username = sqlDataReader["username"].ToString();
                }
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
            string strSql = "update article_reply_Tb set reply_txt = @reply_txt, reply_update = @reply_update " +
                "where reply_id = @reply_id";
            SqlParameter[] sqlParameters = new SqlParameter[]{ 
                new SqlParameter("@reply_txt", SqlDbType.NText),
                new SqlParameter("@reply_update", SqlDbType.DateTime),
                new SqlParameter("@reply_id", SqlDbType.Int)
            };
            sqlParameters[0].Value = ArticleReplyInfo.reply_txt;
            sqlParameters[1].Value = DateTime.Now;
            sqlParameters[2].Value = ArticleReplyInfo.reply_id;
            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 0;
        }

        public bool DeleteArticleReply_Md(int articleReplyId)
        {
            article_reply_Tb objItem = GetArticleReply_Md(articleReplyId);
            db.article_reply_Tb.Remove(objItem);
            return db.SaveChanges() > 0;
        }
        #endregion
        #endregion
    }
}