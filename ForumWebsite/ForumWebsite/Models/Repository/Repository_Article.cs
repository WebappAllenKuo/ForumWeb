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
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        public List<article_Tb> ListArticle_Md(int? theme)
        {
            List<article_Tb> objItems = new List<article_Tb>();
            string strSql = "";
            if (theme == null)
            {
                strSql = "select * from article_tb";
            }
            else
            {
                strSql = "select * from article_tb where a.arti_theme = " + theme;
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
                            user_id = int.Parse(sqlDataReader["user_id"].ToString())
                        });
                    }
                }
            }
            return objItems;
        }
        #endregion
        #region 文章回覆
        public article_reply_Tb GetArticleReply(int? id)
        {
            article_reply_Tb item = new article_reply_Tb();
            //判定是否有傳遞文章編號
            if(id == null)
            {
                return item;
            }
            string strSql = "select * from article_reply_Tb where arti_id = @id";
            SqlParameter[] objSqlParameter = new SqlParameter[]{
                new SqlParameter("@id", SqlDbType.Int)
            };
            objSqlParameter[0].Value = id;
            using (SqlDataReader sqlDataReader = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, objSqlParameter))
            {
                if (sqlDataReader.Read()) {
                    item.reply_id = int.Parse(sqlDataReader[""].ToString());
                    item.reply_txt = sqlDataReader[""].ToString();
                    item.reply_date = DateTime.Parse(sqlDataReader[""].ToString());
                    item.reply_update = DateTime.Parse(sqlDataReader[""].ToString());
                    item.arti_id = int.Parse(sqlDataReader[""].ToString());
                    item.user_id = int.Parse(sqlDataReader[""].ToString());
                }
            }
            return item;
        }

        public int GetArticleReplyCount(int id)
        {
            return db.article_reply_Tb.Count();
        }

        public List<article_reply_Tb> ListArticleReply(int id)
        {
            List<article_reply_Tb> objItems = new List<article_reply_Tb>();
            string strSql = "select * from article_reply_Tb where arti_id = @id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", SqlDbType.Int)
            };
            sqlParameters[0].Value = id;

            using (SqlDataReader sqlDataReader = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters))
            {
                objItems.Add(new article_reply_Tb() {
                    reply_id = int.Parse(sqlDataReader["reply_id"].ToString()),
                    reply_txt = sqlDataReader["reply_txt"].ToString(),
                    reply_date = DateTime.Parse(sqlDataReader["reply_date"].ToString()),
                    reply_update = DateTime.Parse(sqlDataReader["reply_update"].ToString()),
                    arti_id = int.Parse(sqlDataReader["arti_id"].ToString()),
                    user_id = int.Parse(sqlDataReader["user_id"].ToString())
                });
            }
            return objItems;
        }

        public bool InsertArticle(int userId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateArticle(int articleId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteArticle(int articleId)
        {
            throw new NotImplementedException();
        }

        public bool InsertArticleReply(article_reply_Tb addArticleReplyInfo)
        {
            throw new NotImplementedException();
        }

        public bool UpdateArticleReply(int articleReplyId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteArticleReply(int articleReplyId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}