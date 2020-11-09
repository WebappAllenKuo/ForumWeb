using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models;

namespace ForumWebsite.Models.Interface.Repository
{
    public interface IRepository_Board
    {
        /// <summary>
        /// 獲得討論版選單列表
        /// </summary>
        /// <returns></returns>
        IList<board_Tb> ListBoard_Md();
        /// <summary>
        /// 討論版數量
        /// </summary>
        /// <returns></returns>
        int GetBoardCount();
        /// <summary>
        /// 討論版名稱
        /// </summary>
        /// <param name="id">討論版編號</param>
        /// <returns></returns>
        string GetBoardName(int id);

        #region 資料異動
        /// <summary>
        /// 新增討論版
        /// </summary>
        /// <param name="addBoardInfo">新增討論版資訊</param>
        /// <returns></returns>
        bool InsertBoard(board_Tb addBoardInfo);
        /// <summary>
        /// 更新討論版
        /// </summary>
        /// <param name="boardId">討論版編號</param>
        /// <returns></returns>
        bool UpdateBoard(int boardId);
        /// <summary>
        /// 刪除討論版
        /// </summary>
        /// <param name="boardId">討論版編號</param>
        /// <returns></returns>
        bool DeleteBoard(int boardId);
        #endregion
    }
}