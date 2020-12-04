using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumWebsite.Models.Interface.Services
{
    public interface IBoardService
    {
        #region 討論版
        /// <summary>
        /// 獲得討論版選單列表
        /// </summary>
        /// <returns></returns>
        List<board_Tb> ListBoard_Md();
        /// <summary>
        /// 獲得討論版資訊
        /// </summary>
        /// <returns></returns>
        board_Tb BoardItem_Md(int? boardId, int? themeId);
        /// <summary>
        /// 討論版數量
        /// </summary>
        /// <returns></returns>
        int GetBoardCount_Md();
        /// <summary>
        /// 討論版名稱
        /// </summary>
        /// <param name="id">ID編號</param>
        /// <returns></returns>
        string GetBoardName_Md(int id);

        #region 討論版-資料異動
        /// <summary>
        /// 新增討論版
        /// </summary>
        /// <param name="addBoardInfo">新增討論版資訊</param>
        /// <returns></returns>
        bool InsertBoard_Md(board_Tb addBoardInfo);
        /// <summary>
        /// 更新討論版
        /// </summary>
        /// <param name="boardId">討論版資訊</param>
        /// <returns></returns>
        bool UpdateBoard_Md(board_Tb BoardInfo);
        /// <summary>
        /// 刪除討論版
        /// </summary>
        /// <param name="boardId">討論版編號</param>
        /// <returns></returns>
        bool DeleteBoard_Md(int boardId);
        #endregion
        #endregion

        #region 討論版分類
        /// <summary>
        /// 各討論版區塊分類集
        /// </summary>
        /// <param name="boardId">討論版編號</param>
        /// <returns></returns>
        List<board_Tb> ListTheme_Md(int boardId);
        /// <summary>
        /// 獲取各討論版區塊分類數量
        /// </summary>
        /// <param name="boardId">討論版編號</param>
        /// <returns></returns>
        int GetThemeCount_Md(int boardId);
        /// <summary>
        /// 獲取討論版區塊分類的討論版編號
        /// </summary>
        /// <param name="themeId">分類編號</param>
        /// <returns></returns>
        /// theme_board_id 就是討論版的編號
        int GetBoardIdOfTheme_Md(int themeId);
        /// <summary>
        /// 分類名稱
        /// </summary>
        /// <param name="id">ID編號</param>
        /// <returns></returns>
        string GetThemeName_Md(int id);
        #region 討論版分類-資料異動
        /// <summary>
        /// 新增討論版
        /// </summary>
        /// <param name="addBoardInfo">新增討論版資訊</param>
        /// <returns></returns>
        bool InsertTheme_Md(board_Tb addBoardInfo);
        /// <summary>
        /// 更新討論版
        /// </summary>
        /// <param name="boardId">討論版資訊</param>
        /// <returns></returns>
        bool UpdateTheme_Md(board_Tb BoardInfo);
        /// <summary>
        /// 刪除討論版
        /// </summary>
        /// <param name="boardId">討論版編號</param>
        /// <returns></returns>
        bool DeleteTheme_Md(int boardId);
        #endregion
        #endregion
    }
}