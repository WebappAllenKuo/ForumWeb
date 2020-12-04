using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ForumWebsite.Models;
using ForumWebsite.Models.Interface.Repositories;
using Dapper;
using ForumWebsite.Connection;

namespace Repositories.Forum
{
    public class BoardRepoBiz : IBoardRepo
    {
        ForumWebsiteDBEntities db = new ForumWebsiteDBEntities();
        ConnectionFactory _connectionFactory = new ConnectionFactory();
        private IDbConnection conn = null;
        public BoardRepoBiz()
        {
            conn = _connectionFactory.CreateConnection();
        }
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
            
            return (int)conn.ExecuteScalar(strSql);
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
            var newboard = new board_Tb
            {
                board_name = addBoardInfo.board_name
            };
            return conn.Execute(strSql, newboard) > 0;
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
            return conn.Execute(strSql, new { id = boardId }) > 0;
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
            board_Tb result = conn.Query<board_Tb>(strSql, new { id = themeId }).FirstOrDefault();
            id = result.id;
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
            
            board_Tb boardItem = new board_Tb
            {
                theme_board_id = addBoardInfo.theme_board_id,
                theme_name = addBoardInfo.theme_name
            };
            return conn.Execute(strSql, boardItem) > 0;
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
            return conn.Execute(strSql, new { id = boardId }) > 0;
        }
        #endregion
        #endregion
    }
}