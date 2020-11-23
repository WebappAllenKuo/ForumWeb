using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ForumWebsite.Models;
using ForumWebsite.Models.Interface.Repository;
using ForumWebsite.Models.SQLHelper;

namespace ForumWebsite.Models.Repository
{
    public class Repository_Board : IRepository_Board
    {
        ForumWebsiteDBEntities db = new ForumWebsiteDBEntities();
        #region 討論版
        /// <summary>
        /// 獲得討論版選單列表
        /// </summary>
        /// <returns></returns>
        public List<board_Tb> ListBoard_Md()
        {
            List<board_Tb> objItem = db.board_Tb.Where(m => m.theme_board_id == 0).OrderByDescending(m => m.id).ToList();
            return objItem;
        }
        /// <summary>
        /// 獲得討論版資訊
        /// </summary>
        /// <returns></returns>
        public board_Tb BoardItem_Md(int? boardId, int? themeId)
        {
            board_Tb objItem = new board_Tb();

            if (boardId != null && themeId != null)
            {
                objItem = db.board_Tb.FirstOrDefault(m => m.id == themeId && m.theme_board_id == boardId);
            }
            else if (boardId != null)
            {
                objItem = db.board_Tb.FirstOrDefault(m => m.id == boardId);
            }else if (themeId != null)
            {
                objItem = db.board_Tb.FirstOrDefault(m => m.id == themeId);
            }
            return objItem;
        }

        public int GetBoardCount_Md()
        {
            string strSql = "select count(*) from board_tb where theme_board_id != 0";
            return (int)SqlHelper.ExecuteScalar(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, null);
        }
        public string GetBoardName_Md(int id)
        {
            string name = db.board_Tb.FirstOrDefault(s => s.id == id && s.theme_board_id == 0).board_name.ToString();
            if (name == null)
            {
                return "找不到討論版資料!!";
            }
            return name;
        }
        #region 討論版-資料變更
        public bool InsertBoard_Md(board_Tb addBoardInfo)
        {
            string strSql = "insert into board_tb (board_name) values(@board_name)";
            SqlParameter[] sqlParameters = new SqlParameter[]{
                new SqlParameter("@board_name", SqlDbType.NVarChar)
            };
            sqlParameters[0].Value = addBoardInfo.board_name;
            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 0;
        }

        public bool UpdateBoard_Md(board_Tb BoardInfo)
        {
            board_Tb objItem = db.board_Tb.FirstOrDefault(m => m.id == BoardInfo.id && m.theme_board_id == 0);
            objItem.board_name = BoardInfo.board_name.ToString();
            return db.SaveChanges() > 0;
        }

        public bool DeleteBoard_Md(int boardId)
        {
            string strSql = "delete from board_tb where id = @id and theme_board_id = 0";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", SqlDbType.Int)
            };
            sqlParameters[0].Value = boardId;
            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 0;
        }
        #endregion
        #endregion
        #region 討論分類
        public List<board_Tb> ListTheme_Md(int boardId)
        {
            List<board_Tb> objItems = db.board_Tb.Where(m => m.theme_board_id == boardId).ToList();
            return objItems;
        }

        public int GetThemeCount_Md(int boardId)
        {
            int num = db.board_Tb.Count(m => m.theme_board_id == boardId);
            return num;
        }
        /// <summary>
        /// 獲取討論版區塊分類的討論版編號
        /// </summary>
        /// <param name="themeId">分類編號</param>
        /// <returns></returns>
        /// theme_board_id 就是討論版的編號
        public int GetBoardIdOfTheme_Md(int themeId)
        {
            int id = 0;
            string strSql = "select theme_board_id from board_tb where id = @id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", SqlDbType.Int)
            };
            sqlParameters[0].Value = themeId;
            using (SqlDataReader sqlDataReader = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters))
            {
                if (sqlDataReader.Read())
                {
                    id = int.Parse(sqlDataReader["theme_board_id"].ToString());
                }
            }
            return id;
        }
        public string GetThemeName_Md(int id)
        {
            string name = db.board_Tb.FirstOrDefault(s => s.id == id && s.theme_board_id != 0).ToString();
            if (name == null)
            {
                return "找不到分類資料!!";
            }
            return name;
        }
        #region 討論版分類-資料變更
        public bool InsertTheme_Md(board_Tb addBoardInfo)
        {
            string strSql = "insert into board_tb (theme_board_id, theme_name) values(@theme_board_id, @theme_name)";
            SqlParameter[] sqlParameters = new SqlParameter[]{
                //插入為討論版id
                new SqlParameter("@theme_board_id", SqlDbType.NVarChar),
                new SqlParameter("@theme_name", SqlDbType.NVarChar)
            };
            sqlParameters[0].Value = addBoardInfo.theme_board_id;
            sqlParameters[1].Value = addBoardInfo.theme_name;
            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 0;
        }

        public bool UpdateTheme_Md(board_Tb BoardInfo)
        {
            board_Tb objItem = db.board_Tb.FirstOrDefault(m => m.id == BoardInfo.id && m.theme_board_id != 0);
            objItem.theme_name = BoardInfo.theme_name.ToString();
            return db.SaveChanges() > 0;
        }

        public bool DeleteTheme_Md(int boardId)
        {
            string strSql = "delete from board_tb where id = @id and theme_board_id != 0";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", SqlDbType.Int)
            };
            sqlParameters[0].Value = boardId;
            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 0;
        }
        #endregion
        #endregion
    }
}