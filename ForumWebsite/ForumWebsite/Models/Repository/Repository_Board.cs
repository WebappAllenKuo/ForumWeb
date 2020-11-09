using System;
using System.Collections.Generic;
using System.Data;
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
        public IList<board_Tb> ListBoard_Md()
        {
            IList<board_Tb> objItem = db.board_Tb.OrderByDescending(m => m.board_ID).ToList();
            return objItem;
        }

        public int GetBoardCount()
        {
            string strSql = "select count(*) from board_tb";
            return (int)SqlHelper.ExecuteScalar(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, null);
        }

        public string GetBoardName(int id)
        {
            string name = db.board_Tb.FirstOrDefault(s => s.board_ID == id).ToString();
            if (name == null)
            {
                return "找不到資料!!";
            }
            return name;
        }

        public bool InsertBoard(board_Tb addBoardInfo)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBoard(int boardId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteBoard(int boardId)
        {
            throw new NotImplementedException();
        }
    }
}