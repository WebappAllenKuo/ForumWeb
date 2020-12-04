using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models;
using ForumWebsite.Models.Interface.Repositories;
using ForumWebsite.Models.Interface.Services;
using Repositories.Forum;

namespace Services.Forum
{
    public class BoardServiceBiz : IBoardService
    {
        protected IBoardRepo Repository_Board_P { get; private set; }
        public BoardServiceBiz() : this(null) { }
        public BoardServiceBiz(IBoardRepo Repository_Board_Val)
        {
            Repository_Board_P = Repository_Board_Val ?? new BoardRepoBiz();
        }
        #region 討論版
        public List<board_Tb> ListBoard_Md()
        {
            return Repository_Board_P.ListBoard_Md();
        }
        /// <summary>
        /// 獲得討論版資訊
        /// </summary>
        /// <returns></returns>
        public board_Tb BoardItem_Md(int? boardId, int? themeId) {
            return Repository_Board_P.BoardItem_Md(boardId, themeId);
        }
        public int GetBoardCount_Md()
        {
            return Repository_Board_P.GetBoardCount_Md();
        }
        public string GetBoardName_Md(int id)
        {
            return Repository_Board_P.GetBoardName_Md(id);
        }
        #region 討論版-資料異動
        public bool InsertBoard_Md(board_Tb addBoardInfo)
        {
            return Repository_Board_P.InsertBoard_Md(addBoardInfo);
        }

        public bool UpdateBoard_Md(board_Tb BoardInfo)
        {
            return Repository_Board_P.UpdateBoard_Md(BoardInfo);
        }

        public bool DeleteBoard_Md(int boardId)
        {
            return Repository_Board_P.DeleteBoard_Md(boardId);
        }
        #endregion
        #endregion

        #region 討論版分類
        public List<board_Tb> ListTheme_Md(int boardId)
        {
            return Repository_Board_P.ListTheme_Md(boardId);
        }
        public int GetThemeCount_Md(int boardId)
        {
            return Repository_Board_P.GetThemeCount_Md(boardId);
        }
        public int GetBoardIdOfTheme_Md(int themeId)
        {
            return Repository_Board_P.GetBoardIdOfTheme_Md(themeId);
        }
        public string GetThemeName_Md(int id)
        {
            return Repository_Board_P.GetThemeName_Md(id);
        }
        #region 討論版分類-資料異動
        public bool InsertTheme_Md(board_Tb addBoardInfo)
        {
            return Repository_Board_P.InsertTheme_Md(addBoardInfo);
        }

        public bool UpdateTheme_Md(board_Tb BoardInfo)
        {
            return Repository_Board_P.UpdateBoard_Md(BoardInfo);
        }

        public bool DeleteTheme_Md(int boardId)
        {
            return Repository_Board_P.DeleteBoard_Md(boardId);
        }
        #endregion
        #endregion
    }
}